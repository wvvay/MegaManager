using MegaManager.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaManager.Models
{
    public class Entry
    {
        public int Id { get; set; }

        //[ForeignKey("User")]
        public string IdUser { get; set; }
        public string URL { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }

        // Навигационное свойство для связи с пользователем
        public virtual ApplicationUser User { get; set; }
    }
}
