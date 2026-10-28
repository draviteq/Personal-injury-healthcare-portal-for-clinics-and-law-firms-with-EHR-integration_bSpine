using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text;

namespace CapitalBraineAndSpine.Services.Infrastructures.Helpers
{
    public class TrackChangesHelper
    {
        public static string GetChanges(EntityEntry entity)
        {
            // Initialize a StringBuilder for accumulating changes
            var changes = new StringBuilder();

            // Define a set of property names to exclude (case-insensitive comparison)
            var excludedProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Timestamp",
                    "CreatedDate",
                    "UpdatedDate",
                    "ParentUserId"
                };

            // Iterate through all properties in the entity's original values
            foreach (var property in entity.OriginalValues.Properties)
            {
                // Skip excluded properties
                if (excludedProperties.Contains(property.Name))
                    continue;

                // Get original and current values, handling nulls gracefully
                var originalValue = entity.OriginalValues[property]?.ToString() ?? string.Empty;
                var currentValue = entity.CurrentValues[property]?.ToString() ?? string.Empty;

                // Check if values have changed
                if (!string.Equals(originalValue, currentValue, StringComparison.Ordinal))
                {
                    // Append the change details to the StringBuilder
                    if (string.IsNullOrEmpty(originalValue))
                        changes.AppendLine($"{property.Name}: {currentValue} \r");
                    else if (string.IsNullOrEmpty(currentValue))
                        changes.AppendLine($"{property.Name}: {originalValue} (removed) \r");
                    else
                        changes.AppendLine($"{property.Name}: {originalValue} --> {currentValue}");

                }
            }

            // Return the accumulated changes as a string
            return changes.ToString();
        }

    }
}
