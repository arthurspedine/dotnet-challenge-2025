namespace Motoflow.Web.Models.Common
{
    public class HateoasLinks
    {
        public Dictionary<string, string> Links { get; set; } = [];

        public void AddLink(string rel, string href, string method = "GET")
        {
            Links[rel] = href;
        }

        public void AddSelfLink(string href)
        {
            AddLink("self", href);
        }

        public void AddEditLink(string href)
        {
            AddLink("edit", href, "PUT");
        }

        public void AddDeleteLink(string href)
        {
            AddLink("delete", href, "DELETE");
        }

        public void AddCollectionLink(string href)
        {
            AddLink("collection", href);
        }

        public void AddPaginationLinks(string baseUrl, int currentPage, int totalPages, int pageSize)
        {
            AddLink("first", $"{baseUrl}?page=1&pageSize={pageSize}");
            AddLink("last", $"{baseUrl}?page={totalPages}&pageSize={pageSize}");

            if (currentPage > 1)
            {
                AddLink("prev", $"{baseUrl}?page={currentPage - 1}&pageSize={pageSize}");
            }

            if (currentPage < totalPages)
            {
                AddLink("next", $"{baseUrl}?page={currentPage + 1}&pageSize={pageSize}");
            }
        }
    }

    public abstract class HateoasResource
    {
        public Dictionary<string, string> Links { get; set; } = [];
    }
}