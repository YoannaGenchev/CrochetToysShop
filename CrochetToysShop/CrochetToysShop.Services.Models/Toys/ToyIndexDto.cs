using CrochetToysShop.Services.Models.Common;

namespace CrochetToysShop.Services.Models.Toys
{
    public class ToyIndexDto
    {
        public IEnumerable<ToyListItemDto> Toys { get; set; } = new List<ToyListItemDto>();

        public int? CategoryId { get; set; }

        public IEnumerable<CategoryOptionDto> Categories { get; set; } = new List<CategoryOptionDto>();

        public PaginationDto Pagination { get; set; } = new PaginationDto();
    }
}
