using System.Collections.Generic;

namespace OnlineDemonstrator.Libraries.Domain.Dto
{
    public class PosterListOut
    {
        public List<PosterOut> Posters { get; set; }

        public int PostersCount { get; set; }
    }
}