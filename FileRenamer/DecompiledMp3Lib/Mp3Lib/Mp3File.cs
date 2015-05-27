using System.IO;
using DecompiledMp3Lib.Id3Lib;

namespace DecompiledMp3Lib.Mp3Lib
{
    public class Mp3File
    {
        private FileInfo _sourceFileInfo;
        private Mp3FileData _mp3FileData;

        private Mp3FileData Mp3FileData
        {
            get
            {
                if (this._mp3FileData == null)
                    this._mp3FileData = new Mp3FileData(this._sourceFileInfo);
                return this._mp3FileData;
            }
        }

        public string FileName
        {
            get { return this._sourceFileInfo.FullName; }
        }

        public TagModel TagModel
        {
            get { return this.Mp3FileData.TagModel; }
            set { this.Mp3FileData.TagModel = value; }
        }

        public TagHandler TagHandler
        {
            get { return this.Mp3FileData.TagHandler; }
            set { this.Mp3FileData.TagHandler = value; }
        }

        public Mp3File(string file)
            : this(new FileInfo(file))
        {
        }

        public Mp3File(FileInfo fileinfo)
        {
            this._sourceFileInfo = fileinfo;
        }

        public void Update()
        {
            if (this._mp3FileData == null || this._mp3FileData.Update() != Mp3FileData.CacheDataState.eDirty)
                return;
            this._mp3FileData = (Mp3FileData) null;
        }

        public void UpdatePacked()
        {
            if (this.Mp3FileData.UpdatePacked() != Mp3FileData.CacheDataState.eDirty)
                return;
            this._mp3FileData = (Mp3FileData) null;
        }

        public void UpdateNoV2tag()
        {
            if (this.Mp3FileData.UpdateNoV2tag() != Mp3FileData.CacheDataState.eDirty)
                return;
            this._mp3FileData = (Mp3FileData) null;
        }
    }
}
