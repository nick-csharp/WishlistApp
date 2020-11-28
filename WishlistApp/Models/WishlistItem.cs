using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WishlistApp.Models
{
    public class WishlistItem
	{
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }

		[JsonProperty(PropertyName = "userId")]
		public string UserId { get; set; }

		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }

		[JsonProperty(PropertyName = "createdDate")]
		public string CreatedDate { get; set; }

		[JsonProperty(PropertyName = "claimedByUserId")]
		public string ClaimedByUserId { get; set; }

		public WishlistItem()
        {

        }

		public WishlistItem(string userId, string description)
        {
			Id = Guid.NewGuid().ToString();
			UserId = userId;
			Description = description;
			CreatedDate = DateTime.Now.ToString();
			ClaimedByUserId = null;
        }

		public WishlistItem(WishlistItemDto dto)
        {
			Id = dto.Id;
			UserId = dto.UserId;
			Description = dto.Description;
        }
    }
}
