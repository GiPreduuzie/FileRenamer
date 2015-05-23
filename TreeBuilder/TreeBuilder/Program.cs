using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeBuilder
{
    public interface INode
    {
        IEnumerable<INode> Children { get; }
    }

    public class Root : INode
    {
        public Root(IEnumerable<Folder> folders, IEnumerable<Item> items)
        {
            Items = items;
            Folders = folders;
        }

        public IEnumerable<Item> Items { get; private set; }
        public IEnumerable<Folder> Folders { get; private set; }

        public IEnumerable<INode> Children
        {
            get { return Folders.Cast<INode>().Union(Items); }
        }

        public override string ToString()
        {
            return FormatHelper.Join(
                "[root:",
                FormatHelper.JoinAndIdent(Folders),
                FormatHelper.JoinAndIdent(Items),
                "]");
        }
    }

    public class Folder : INode
    {
        public Folder(string name, IEnumerable<Folder> folders, IEnumerable<Item> items)
        {
            Name = name;
            Items = items;
            Folders = folders;
        }

        public string Name {get; private set; }
        public IEnumerable<Item> Items { get; private set; }
        public IEnumerable<Folder> Folders { get; private set; }

        public IEnumerable<INode> Children
        {
            get { return Folders.Cast<INode>().Union(Items); }
        }

        public override string ToString()
        {
            return FormatHelper.Join(
                "[folder: " + Name,
                FormatHelper.JoinAndIdent(Folders),
                FormatHelper.JoinAndIdent(Items),
                "]");
        }
    }

    public class Item : INode
    {
        public Item(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }

        public IEnumerable<INode> Children
        {
            get { return new List<INode>(); }
        }

        public override string ToString()
        {
            return string.Format("[item: {0}, {1}]", Name, string.IsNullOrWhiteSpace(Description)? "<no description>" : Description);
        }
    }

    public static class FormatHelper
    {
        public static string AddIdent(string text)
        {
            var ident = "     ";
            return ident + text.Replace("\r\n", "\r\n" + ident);
        }

        public static string JoinAndIdent(IEnumerable<object> items)
        {
            return FormatHelper.AddIdent(string.Join("\r\n", items));
        }

        public static string Join(params string[] items)
        {
            return string.Join("\r\n", items.Where(x => !string.IsNullOrWhiteSpace(x)));
        }
    }



    public class RootBuilder
    {
        private readonly List<Item> _items = new List<Item>();
        private readonly List<Folder> _folders = new List<Folder>();
        
        public FolderBuilder<RootBuilder> AddFolder(string name)
        {
            return new FolderBuilder<RootBuilder>(HostFolder, name);
        }

        public ItemBuilder<RootBuilder> AddItem(string name)
        {
            return new ItemBuilder<RootBuilder>(HostItem, name);
        }

        private RootBuilder HostItem(Item item)
        {
            _items.Add(item);
            return this;
        }

        private RootBuilder HostFolder(Folder folder)
        {
            _folders.Add(folder);
            return this;
        }

        public Root Done()
        {
            return new Root(_folders, _items);
        }
    }

    public class FolderBuilder<T>
    {
        private readonly List<Item> _items = new List<Item>();
        private readonly List<Folder> _folders = new List<Folder>();
        private readonly Func<Folder, T> _toHost;
        private readonly string _name;

        public FolderBuilder(Func<Folder, T> toHost, string name)
        {
            _name = name;
            _toHost = toHost;
        }

        public FolderBuilder<FolderBuilder<T>> AddFolder(string name)
        {
            return new FolderBuilder<FolderBuilder<T>>(HostFolder, name);
        }

        public ItemBuilder<FolderBuilder<T>> AddItem(string name)
        {
            return new ItemBuilder<FolderBuilder<T>>(HostItem, name);
        }

        private FolderBuilder<T> HostItem(Item item)
        {
            _items.Add(item);
            return this;
        }

        private FolderBuilder<T> HostFolder(Folder folder)
        {
            _folders.Add(folder);
            return this;
        }
 
        public T Done()
        {
            return _toHost(new Folder(_name, _folders, _items));
        }
    }

    public class ItemBuilder<T>
    {
        private string _name;
        private string _description = "";
        private Func<Item, T> _toHost;

        public ItemBuilder(Func<Item, T> toHost, string name)
        {
            _name = name;
            _toHost = toHost;
        }

        public ItemBuilder<T> WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public T Done()
        {
            return _toHost(new Item(_name, _description));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Root root =
                 new RootBuilder()
                     .AddFolder("First folder")
                         .AddFolder("Embedded folder")
                             .AddItem("1").Done()
                             .AddItem("2").WithDescription("second item in embedded folder").Done()
                             .Done()
                         .Done()
                     .AddFolder("Second folder")
                         .AddItem("1").WithDescription("some item").Done()
                         .AddItem("2").Done()
                         .Done()
                     .Done();

            Console.WriteLine(root);

            //That is an output:

            //[root:
            //     [folder: First folder
            //          [folder: Embedded folder
            //               [item: 1, <no description>]
            //               [item: 2, second item in embedded folder]
            //          ]
            //     ]
            //     [folder: Second folder
            //          [item: 1, some item]
            //          [item: 2, <no description>]
            //     ]
            //]
        }
    }
}
