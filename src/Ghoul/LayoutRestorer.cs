using PeanutButter.INIFile;

namespace Ghoul
{
    internal class LayoutRestorer
    {
        private readonly INIFile _config;
        private readonly IApplicationRestarter _applicationRestarter;
        private readonly ISectionNameHelper _sectionNameHelper;

        public LayoutRestorer(
            INIFile config,
            IApplicationRestarter applicationRestarter,
            ISectionNameHelper sectionNameHelper
        )
        {
            _config = config;
            _applicationRestarter = applicationRestarter;
            _sectionNameHelper = sectionNameHelper;
        }

        public void RestoreLayout(string name)
        {
            _applicationRestarter.RestartApplicationsForLayout(name);
            RestoreWindowPositionsForLayout(name);
        }

        private void RestoreWindowPositionsForLayout(string name)
        {
        }
    }
}