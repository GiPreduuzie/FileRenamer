using System.IO;
using DecompiledMp3Lib.Id3Lib;
using DecompiledMp3Lib.Id3Lib.Exceptions;

namespace DecompiledMp3Lib.Mp3Lib
{
    internal class Mp3FileData
    {
        private FileInfo _sourceFileInfo;
        private bool _audioReplaced;
        private uint _audioStart;
        private IAudio _audio;
        private TagHandler _tagHandler;

        public IAudio Audio
        {
            get
            {
                return this._audio;
            }
            set
            {
                this._audio = value;
                this._audioReplaced = true;
            }
        }

        public TagModel TagModel
        {
            get
            {
                return this._tagHandler.FrameModel;
            }
            set
            {
                this._tagHandler.FrameModel = value;
            }
        }

        public TagHandler TagHandler
        {
            get
            {
                return this._tagHandler;
            }
            set
            {
                this._tagHandler = value;
            }
        }

        public Mp3FileData(FileInfo fileinfo)
        {
            this._sourceFileInfo = fileinfo;
            TagModel frameModel = new TagModel();
            uint payloadNumBytes;
            using (FileStream fileStream = fileinfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fileStream.Length > (long)uint.MaxValue)
                    throw new InvalidAudioFrameException("MP3 file can't be bigger than 4gb");
                this._audioStart = 0U;
                payloadNumBytes = (uint)fileStream.Length;
                try
                {
                    ID3v1 id3v1 = new ID3v1();
                    id3v1.Deserialize((Stream)fileStream);
                    frameModel = id3v1.FrameModel;
                    payloadNumBytes -= ID3v1.TagLength;
                }
                catch (TagNotFoundException ex)
                {
                }
                try
                {
                    fileStream.Seek(0L, SeekOrigin.Begin);
                    frameModel = TagManager.Deserialize((Stream)fileStream);
                    this._audioStart = (uint)fileStream.Position;
                    payloadNumBytes -= this._audioStart;
                }
                catch (TagNotFoundException ex)
                {
                }
                this._tagHandler = new TagHandler(frameModel);
            }
            this._audio = (IAudio)new AudioFile(fileinfo, this._audioStart, payloadNumBytes, this._tagHandler.Length);
            this._audioReplaced = false;
        }

        public Mp3FileData.CacheDataState Update()
        {
            if (!this.TagModel.IsValid)
                return this.UpdateNoV2tag();
            this.TagModel.UpdateSize();
            uint withHeaderFooter = this.TagModel.Header.TagSizeWithHeaderFooter;
            if (withHeaderFooter <= this._audioStart && !this._audioReplaced)
            {
                this.UpdateInSitu(withHeaderFooter);
                return Mp3FileData.CacheDataState.eClean;
            }
            uint num = withHeaderFooter + this._audio.NumPayloadBytes + ID3v1.TagLength;
            this.TagModel.Header.PaddingSize = (uint)((int)num + 2047 & -2048) - num;
            this.RewriteFile(new FileInfo(Path.ChangeExtension(this._sourceFileInfo.FullName, "bak")));
            return Mp3FileData.CacheDataState.eDirty;
        }

        public Mp3FileData.CacheDataState UpdatePacked()
        {
            if (this.TagModel.Count == 0)
                return this.UpdateNoV2tag();
            this.TagModel.UpdateSize();
            this.TagModel.Header.PaddingSize = 0U;
            this.RewriteFile(new FileInfo(Path.ChangeExtension(this._sourceFileInfo.FullName, "bak")));
            return Mp3FileData.CacheDataState.eDirty;
        }

        public Mp3FileData.CacheDataState UpdateNoV2tag()
        {
            if ((int)this._audioStart == 0 && !this._audioReplaced)
            {
                this.UpdateInSituNoV2tag();
                return Mp3FileData.CacheDataState.eClean;
            }
            this.RewriteFileNoV2tag(new FileInfo(Path.ChangeExtension(this._sourceFileInfo.FullName, "bak")));
            return Mp3FileData.CacheDataState.eDirty;
        }

        private void UpdateInSitu(uint tagSizeComplete)
        {
            this.TagModel.Header.PaddingSize = this._audioStart - tagSizeComplete;
            using (FileStream fileStream = this._sourceFileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                TagManager.Serialize(this.TagModel, (Stream)fileStream);
                this.WriteID3v1((Stream)fileStream);
            }
        }

        private void RewriteFile(FileInfo bakFileInfo)
        {
            FileInfo sourceLocation = new FileInfo(Path.ChangeExtension(this._sourceFileInfo.FullName, "$$$"));
            using (FileStream writeStream = sourceLocation.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                TagManager.Serialize(this.TagModel, (Stream)writeStream);
                uint num = (uint)writeStream.Position;
                this.CopyAudioStream(writeStream);
                this._audioStart = num;
                this.WriteID3v1((Stream)writeStream);
            }
            try
            {
                FileMover.FileMover.FileMove(sourceLocation, this._sourceFileInfo, bakFileInfo);
            }
            catch
            {
                sourceLocation.Delete();
                throw;
            }
        }

        private void UpdateInSituNoV2tag()
        {
            using (FileStream fileStream = this._sourceFileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                this.WriteID3v1((Stream)fileStream);
        }

        private void RewriteFileNoV2tag(FileInfo bakFileInfo)
        {
            FileInfo sourceLocation = new FileInfo(Path.ChangeExtension(this._sourceFileInfo.FullName, "$$$"));
            using (FileStream writeStream = sourceLocation.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                this.CopyAudioStream(writeStream);
                this._audioStart = 0U;
                this.WriteID3v1((Stream)writeStream);
            }
            try
            {
                FileMover.FileMover.FileMove(sourceLocation, this._sourceFileInfo, bakFileInfo);
            }
            catch
            {
                sourceLocation.Delete();
                throw;
            }
        }

        public void CopyAudioStream(FileStream writeStream)
        {
            using (Stream stream = this._audio.OpenAudioStream())
            {
                byte[] buffer = new byte[4096];
                int count;
                while ((count = stream.Read(buffer, 0, 4096)) > 0)
                    writeStream.Write(buffer, 0, count);
            }
        }

        public void WriteID3v1(Stream stream)
        {
            ID3v1 id3v1 = new ID3v1();
            id3v1.FrameModel = this.TagModel;
            stream.Seek((long)(this._audioStart + this._audio.NumPayloadBytes), SeekOrigin.Begin);
            id3v1.Write(stream);
        }

        public enum CacheDataState
        {
            eClean,
            eDirty,
        }
    }
}

