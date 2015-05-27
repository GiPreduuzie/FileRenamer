using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DecompiledMp3Lib.Id3Lib.Frames;

namespace DecompiledMp3Lib.Id3Lib
{
    public class TagModel : Collection<FrameBase>
    {
        private TagHeader _tagHeader = new TagHeader();
        private TagExtendedHeader _tagExtendedHeader = new TagExtendedHeader();

        public bool IsValid
        {
            get
            {
                return this.Count > 0;
            }
        }

        public TagHeader Header
        {
            get
            {
                return this._tagHeader;
            }
        }

        public TagExtendedHeader ExtendedHeader
        {
            get
            {
                return this._tagExtendedHeader;
            }
        }

        protected override void InsertItem(int index, FrameBase item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.FrameId == null || item.FrameId.Length != 4)
                throw new InvalidOperationException("The frame id is invalid");
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, FrameBase item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.FrameId == null || item.FrameId.Length != 4)
                throw new InvalidOperationException("The frame id is invalid");
            base.SetItem(index, item);
        }

        public void AddRange(IEnumerable<FrameBase> frames)
        {
            if (frames == null)
                throw new ArgumentNullException("frames");
            foreach (FrameBase frameBase in frames)
                base.Add(frameBase);
        }

        public void UpdateSize()
        {
            if (!this.IsValid)
                this.Header.TagSize = 0U;
            using (Stream stream = (Stream)new MemoryStream())
                TagManager.Serialize(this, stream);
        }
    }
}