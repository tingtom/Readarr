using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.Indexers.LibGen
{
    public class LibGenSettingsValidator : AbstractValidator<LibGenSettings>
    {
        public LibGenSettingsValidator()
        {
            RuleFor(c => c.BaseUrl).ValidRootUrl();
        }
    }

    public class LibGenSettings : IIndexerSettings
    {
        private static readonly LibGenSettingsValidator Validator = new LibGenSettingsValidator();

        public LibGenSettings()
        {
            BaseUrl = "http://gen.lib.rus.ec/fiction/";
        }

        [FieldDefinition(0, Label = "URL", Advanced = true, HelpText = "")]
        public string BaseUrl { get; set; }
        public int? EarlyReleaseLimit { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
