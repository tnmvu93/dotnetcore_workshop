using ConferenceDTO;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FrontEnd.Pages
{
    public class SessionModel : PageModel
    {
        private readonly IApiClient _apiClient;
        private readonly HtmlEncoder _htmlEncoder;

        public SessionResponse Session { get; set; }
        public int? DayOffset { get; set; }

        public SessionModel(IApiClient apiClient, HtmlEncoder htmlEncoder)
        {
            _apiClient = apiClient;
            _htmlEncoder = htmlEncoder;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Session = await _apiClient.GetSessionAsync(id);

            if (Session == null)
            {
                return RedirectToPage("/Index");
            }

            var allSessions = await _apiClient.GetSessionsAsync();

            var startDate = allSessions.Min(s => s.StartTime?.Date);
                
            DayOffset = Session.StartTime?.DateTime.Subtract(startDate ?? DateTime.MinValue).Days;

            if (!string.IsNullOrEmpty(Session.Abstract))
            {
                var encodedCrLf = _htmlEncoder.Encode("\r\n");
                var encodedAbstract = _htmlEncoder.Encode(Session.Abstract);
                Session.Abstract = "<p>" + String.Join("</p><p>", encodedAbstract.Split(encodedCrLf, StringSplitOptions.RemoveEmptyEntries)) + "</p>";
            }

            return Page();
        }
    }
}