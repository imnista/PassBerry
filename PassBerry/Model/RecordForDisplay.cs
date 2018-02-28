namespace PassBerry.Model
{
    using PassBerry.Library;
    using System;
    using System.Drawing;
    using System.Security;

    public class RecordForDisplay
    {
        public RecordForDisplay()
        {
        }

        public RecordForDisplay(Record data)
        {
            this.Id = data.Id;
            this.Name = data.Name;
            this.PictureForLogo = ImageHelper.GetImageFromBase64String(data.PictureForLogo);
            this.Keywords = data.Keywords;
            this.Username = data.Username;
            this.Password = SecureStringHelper.GetSecureStringFromString(data.Password);
            this.Remarks = data.Remarks;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Image PictureForLogo { get; set; }
        public string Keywords { get; set; }
        public string Username { get; set; }
        public SecureString Password { get; set; }
        public string Remarks { get; set; }

        public Record ToDataModel()
        {
            return new Record
            {
                Id = this.Id,
                Name = this.Name,
                PictureForLogo = ImageHelper.GetBase64StringFromImage(this.PictureForLogo),
                Keywords = this.Keywords,
                Username = this.Username,
                Password = SecureStringHelper.GetStringFromSecureString(this.Password),
                Remarks = this.Remarks
            };
        }
    }
}