using System;
using System.Collections.ObjectModel;
using System.Drawing;
using DecompiledMp3Lib.Id3Lib.Frames;

namespace DecompiledMp3Lib.Id3Lib
{
    public class TagHandler
    {
        private string _language = "eng";
        private TagModel _frameModel;
        private TextCode _textCode;

        public TagModel FrameModel
        {
            get
            {
                return this._frameModel;
            }
            set
            {
                this._frameModel = value;
            }
        }

        public string Song
        {
            get
            {
                return this.Title;
            }
            set
            {
                this.Title = value;
            }
        }

        public string Title
        {
            get
            {
                return this.GetTextFrame("TIT2");
            }
            set
            {
                this.SetTextFrame("TIT2", value);
            }
        }

        public string Artist
        {
            get
            {
                return this.GetTextFrame("TPE1");
            }
            set
            {
                this.SetTextFrame("TPE1", value);
            }
        }

        public string Album
        {
            get
            {
                return this.GetTextFrame("TALB");
            }
            set
            {
                this.SetTextFrame("TALB", value);
            }
        }

        public string Year
        {
            get
            {
                return this.GetTextFrame("TYER");
            }
            set
            {
                this.SetTextFrame("TYER", value);
            }
        }

        public string Composer
        {
            get
            {
                return this.GetTextFrame("TCOM");
            }
            set
            {
                this.SetTextFrame("TCOM", value);
            }
        }

        public string Genre
        {
            get
            {
                return this.GetTextFrame("TCON");
            }
            set
            {
                this.SetTextFrame("TCON", value);
            }
        }

        public string Track
        {
            get
            {
                return this.GetTextFrame("TRCK");
            }
            set
            {
                this.SetTextFrame("TRCK", value);
            }
        }

        public string Disc
        {
            get
            {
                return this.GetTextFrame("TPOS");
            }
            set
            {
                this.SetTextFrame("TPOS", value);
            }
        }

        public TimeSpan? Length
        {
            get
            {
                string textFrame = this.GetTextFrame("TLEN");
                if (string.IsNullOrEmpty(textFrame))
                    return new TimeSpan?();
                int result;
                if (int.TryParse(textFrame, out result))
                    return new TimeSpan?(new TimeSpan(0, 0, 0, 0, result));
                return new TimeSpan?();
            }
        }

        public uint PaddingSize
        {
            get
            {
                return this._frameModel.Header.PaddingSize;
            }
        }

        public string Lyrics
        {
            get
            {
                return this.GetFullTextFrame("USLT");
            }
            set
            {
                this.SetFullTextFrame("USLT", value);
            }
        }

        public string Comment
        {
            get
            {
                return this.GetFullTextFrame("COMM");
            }
            set
            {
                this.SetFullTextFrame("COMM", value);
            }
        }

        public Image Picture
        {
            get
            {
                FramePicture framePicture = this.FindFrame("APIC") as FramePicture;
                if (framePicture == null)
                    return null;
                return framePicture.Picture;
            }
            set
            {
                FramePicture framePicture1 = this.FindFrame("APIC") as FramePicture;
                if (framePicture1 != null)
                {
                    if (value != null)
                        framePicture1.Picture = value;
                    else
                        this._frameModel.Remove((FrameBase)framePicture1);
                }
                else
                {
                    if (value == null)
                        return;
                    FramePicture framePicture2 = FrameFactory.Build("APIC") as FramePicture;
                    framePicture2.Picture = value;
                    this._frameModel.Add((FrameBase)framePicture2);
                }
            }
        }

        public TagHandler(TagModel frameModel)
        {
            this._frameModel = frameModel;
        }

        private void SetTextFrame(string frameId, string message)
        {
            FrameBase frame = this.FindFrame(frameId);
            if (frame != null)
            {
                if (!string.IsNullOrEmpty(message))
                    ((FrameText)frame).Text = message;
                else
                    this._frameModel.Remove(frame);
            }
            else
            {
                if (string.IsNullOrEmpty(message))
                    return;
                FrameText frameText = (FrameText)FrameFactory.Build(frameId);
                frameText.Text = message;
                frameText.TextCode = this._textCode;
                this._frameModel.Add((FrameBase)frameText);
            }
        }

        private string GetTextFrame(string frameId)
        {
            FrameBase frame = this.FindFrame(frameId);
            if (frame != null)
                return ((FrameText)frame).Text;
            return string.Empty;
        }

        private void SetFullTextFrame(string frameId, string message)
        {
            FrameBase frame = this.FindFrame(frameId);
            if (frame != null)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    FrameFullText frameFullText = (FrameFullText)frame;
                    frameFullText.Text = message;
                    frameFullText.TextCode = this._textCode;
                    frameFullText.Description = string.Empty;
                    frameFullText.Language = this._language;
                }
                else
                    this._frameModel.Remove(frame);
            }
            else
            {
                if (string.IsNullOrEmpty(message))
                    return;
                FrameFullText frameFullText = (FrameFullText)FrameFactory.Build(frameId);
                frameFullText.TextCode = this._textCode;
                frameFullText.Language = "eng";
                frameFullText.Description = string.Empty;
                frameFullText.Text = message;
                this._frameModel.Add((FrameBase)frameFullText);
            }
        }

        private string GetFullTextFrame(string frameId)
        {
            FrameBase frame = this.FindFrame(frameId);
            if (frame != null)
                return ((FrameFullText)frame).Text;
            return string.Empty;
        }

        private FrameBase FindFrame(string frameId)
        {
            foreach (FrameBase frameBase in (Collection<FrameBase>)this._frameModel)
            {
                if (frameBase.FrameId == frameId)
                    return frameBase;
            }
            return (FrameBase)null;
        }
    }
}
