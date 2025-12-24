namespace ArmarioLATAM.Components.ClassProviders
{
    using System.Linq;
    using Microsoft.AspNetCore.Components.Forms;
    public class LatamFieldClassProvider : FieldCssClassProvider
    {
        public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
        {
            var hasErrors = editContext.GetValidationMessages(fieldIdentifier).Any();

            // Solo marcar como válido/ inválido si el campo fue tocado
            if (editContext.IsModified(fieldIdentifier))
            {
         
            }

            return "";
        }
    }
}