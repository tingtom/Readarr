using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.Indexers.LibGen
{
    public class LibGenSettings : IIndexerSettings
    {
        [FieldDefinition(0, Label = "URL", Advanced = true, HelpText = "")]
        public string BaseUrl { get; set; }
        public int? EarlyReleaseLimit { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult();
        }
    }
}
