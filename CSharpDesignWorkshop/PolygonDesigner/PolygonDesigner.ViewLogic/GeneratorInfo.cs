using Polygon.Core.Generators;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace PolygonDesigner.ViewLogic
{
    public class GeneratorInfo : BindableBase
    {
        private readonly PolygonManagementViewModel Parent;

        public string FriendlyName { get; }

        public IPolygonGenerator Generator { get; }

        public bool Selected
        {
            get { return Parent.SelectedPolygonGenerator == Generator; }
            set { Parent.SelectedPolygonGenerator = Generator; }
        }

        public GeneratorInfo(PolygonManagementViewModel parent, string friendlyName, IPolygonGenerator generator)
        {
            FriendlyName = friendlyName;
            Generator = generator;
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
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
