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
            BaseUrl = "https://d5ae646747b34928a0f20643afea7a01.europe-west4.gcp.elastic-cloud.com:9243/knowl-libgen-fiction/_search";
        }

        [FieldDefinition(0, Label = "URL", Advanced = true, HelpText = "")]
        public string BaseUrl { get; set; }

        [FieldDefinition(1, Label = "Username", Advanced = true, HelpText = "")]
        public string Username { get; set; }

        [FieldDefinition(2, Label = "Password", Advanced = true, HelpText = "", Type = FieldType.Password)]
        public string Password { get; set; }
        public int? EarlyReleaseLimit { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
