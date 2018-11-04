using Polygon.Core.Generators;
using Prism.Commands;
using Prism.Mvvm;

namespace PolygonDesigner.ViewLogic
{
    public class GeneratorInfo : BindableBase
    {
        private PolygonManagementViewModel Parent;

        public string FriendlyName { get; }

        public PolygonGenerator Generator { get; }

        public bool Selected
        {
            get { return Parent.SelectedPolygonGenerator == Generator; }
            set { Parent.SelectedPolygonGenerator = Generator; }
        }

        public GeneratorInfo(PolygonManagementViewModel parent, string friendlyName, PolygonGenerator generator)
        {
            FriendlyName = friendlyName;
            Generator = generator;
            Parent = parent;
            Parent.PropertyChanged += (_, ea) =>
            {
                if (ea.PropertyName == nameof(PolygonManagementViewModel.SelectedPolygonGenerator))
                {
                    RaisePropertyChanged(nameof(Selected));
                }
            };
        }
    }
}
