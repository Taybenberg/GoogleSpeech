using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoogleSpeechUkrBot
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [DefaultValue(true)]
        public bool Voice { get; set; }

        [DefaultValue(true)]
        public bool Text { get; set; }
    }
}
