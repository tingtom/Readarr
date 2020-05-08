using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.Download.Clients.IPFS
{
    public class IPFSSettingsValidator : AbstractValidator<IPFSSettings>
    {
        public IPFSSettingsValidator()
        {
            RuleFor(a => a.IPFSNodeUrl).IsValidUrl();
        }
    }

    public class IPFSSettings : IProviderConfig
    {
        private static readonly IPFSSettingsValidator Validator = new IPFSSettingsValidator();

        [FieldDefinition(0, Label = "IPFS Node", Type = FieldType.Url, HelpText = "Url to the IPFS node to use, e.g. http://localhost:5001")]
        public string IPFSNodeUrl { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
