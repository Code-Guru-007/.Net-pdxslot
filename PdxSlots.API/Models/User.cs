using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class User
    {
        public User()
        {
            Devices = new HashSet<Device>();
            Events = new HashSet<Event>();
            ZipFileUploads = new HashSet<ZipFileUpload>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string UserIdentityId { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<ZipFileUpload> ZipFileUploads { get; set; }
    }
}
