using ArmarioLATAM.Components.Models;
using System.Collections.Generic;
using System.Linq;

namespace ArmarioLATAM.Services
{
    public class GarmentSelectionState
    {
        public List<GarmentSelection> SelectedGarments { get; } = new();

        public void SetSelections(IEnumerable<GarmentSelection> items)
        {
            SelectedGarments.Clear();
            SelectedGarments.AddRange(items.Where(x => x.Quantity > 0));
        }
        public void Clear()
        {
            SelectedGarments.Clear();
        }
    }
}
