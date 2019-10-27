using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InternetExplorer
{
    class Discovery
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public string Url { get; set; }

        public Discovery(long id, string url)
        {
            Id = id;
            Url = url;
        }
    }
}
