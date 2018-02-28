namespace PassBerry.Model
{
    using System;

    public class Record
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PictureForLogo { get; set; }
        public string Keywords { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Remarks { get; set; }
    }
}