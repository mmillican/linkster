using System.Collections.Generic;
using Linkster.Models.Links;

namespace Linkster.ViewModels.Links
{
    public class ShortLinkListViewModel 
    {
        public List<ShortLink> Links = new List<ShortLink>();

        public NewLinkViewModel NewLink { get; set; } = new NewLinkViewModel();
    }

    public class NewLinkViewModel
    {
        public string DestinationUrl { get; set;}
    }
}