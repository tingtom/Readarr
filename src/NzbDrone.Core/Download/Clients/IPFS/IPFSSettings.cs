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

        public IPFSSettings()
        {
            PinHash = true;
        }

        [FieldDefinition(0, Label = "API Url", Type = FieldType.Url, HelpText = "Url to the IPFS API for the node you want to use, e.g. http://localhost:5001")]
        public string IPFSNodeUrl { get; set; }

        [FieldDefinition(1, Label = "Download Path", Type = FieldType.Path, HelpText = "")]
        public string IPFSDownloadPath { get; set; }

        [FieldDefinition(1, Label = "Pin hash", Type = FieldType.Checkbox, HelpText = "Pin the file to your IPFS node", Advanced = true)]
        public bool PinHash { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
