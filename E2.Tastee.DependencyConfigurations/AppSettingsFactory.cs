using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using E2.Tastee.Common;
using Serilog;

namespace E2.Tastee.DependencyConfigurations
{
    /// <summary>
    /// Creates a settings instance of type T and fills public properties with settings from appsettings.json config files
    /// or environment variables based on naming convention (same name of property for json settings, or uppercase(propertyname) for ENV var settings)
    /// This is used by default with the template generated AppSettings file, but can be used to build other settings files
    /// See attributes in this namespace that can be used to override the conventions or set default values
    /// Evaluation precedence for setting values - ENV vars > json custom section > json "AppSettings" section > default values
    /// </summary>
    public class AppSettingsFactory
    {
        private const string DefaultSettingsSection = "AppSettings";

        public static T CreateAppSettings<T>(IConfigurationRoot configurationRoot) where T : new()
        {
            var defaultSection = configurationRoot?.GetSection(DefaultSettingsSection);
            var appSettings = new T();
            var appSettingsType = typeof(T);
            foreach (var propertyInfo in appSettingsType.GetProperties()
                .Where(x => x.CanWrite && x.GetSetMethod().IsPublic))
            {
                SetPropertyValueFromConfiguration(configurationRoot, defaultSection, appSettings, propertyInfo);
            }
            return appSettings;
        }

        private static void SetPropertyValueFromConfiguration<T>(IConfigurationRoot configurationRoot,
            IConfigurationSection defaultSection, T appSettings, PropertyInfo propertyInfo) where T : new()
        {

            var attributes = propertyInfo.GetCustomAttributes(false);
            var settingName = attributes
                .OfType<SettingNameAttribute>()
                .FirstOrDefault()?.Name 
                              ?? propertyInfo.Name;


            var defaultConfigurationValue = attributes
                .OfType<DefaultAttribute>()
                .FirstOrDefault()?.Value;
            var overrideSectionName = attributes
                                  .OfType<SectionAttribute>()
                                  .FirstOrDefault()?.Name;
            var section = (string.IsNullOrEmpty(overrideSectionName))
                ? defaultSection
                : configurationRoot?.GetSection(overrideSectionName);

            var configStringValue = GetSettingAsString(configurationRoot, section, settingName) 
                                    ?? defaultConfigurationValue;

            if (string.IsNullOrEmpty(configStringValue))
            {
                throw new ConfigurationErrorsException(
                    $"The application setting {settingName} is missing a value");
            }

            try
            {
                if (settingName == "DeveloperModeUserId")
                {
                    var developerModeId = (string.IsNullOrEmpty(configStringValue))
                        ? (int?)null
                        : int.Parse(configStringValue);
                    propertyInfo.SetValue(appSettings, developerModeId);
                }
                else if (propertyInfo.PropertyType.IsGenericType)
                {
                    SetListValues(propertyInfo, appSettings, configStringValue);
                }
                else
                {
                    SetSingleValue<T>(propertyInfo, appSettings, configStringValue);
                }
                
            }
            catch(Exception exc)
            {
                throw new ConfigurationErrorsException(
                    $"The application settings property {settingName} of type {propertyInfo.PropertyType.Name} could not be set with configuration value \"{configStringValue}\" - (#{exc.Message})");
            }
        }

        private static void SetListValues<T>(PropertyInfo propertyInfo, T appSettings, string configStringValue)
        {
            var trimmedStringList = configStringValue
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            switch (propertyInfo.PropertyType.GenericTypeArguments[0].Name)
            {
                case "Int32":
                    var intValues = trimmedStringList
                        .Select(int.Parse)
                        .ToList();
                    propertyInfo.SetValue(appSettings, intValues.ToList(), null);
                    break;
                case "Decimal":
                    var decimalValues = trimmedStringList
                        .Select(decimal.Parse)
                        .ToList();
                    propertyInfo.SetValue(appSettings, decimalValues.ToList(), null);
                    break;
                case "Boolean":
                    var booleanValues = trimmedStringList
                        .Select(bool.Parse)
                        .ToList();
                    propertyInfo.SetValue(appSettings, booleanValues.ToList(), null);
                    break;
                default:
                    var stringValues = trimmedStringList.ToList();
                    propertyInfo.SetValue(appSettings, stringValues.ToList(), null);
                    break;
            }
        }

        private static void SetSingleValue<T>(PropertyInfo propertyInfo, T appSettings, string configStringValue)
        {
            object value = propertyInfo.PropertyType.Name switch
            {
                "Int32" => int.Parse(configStringValue),
                "Decimal" => decimal.Parse(configStringValue),
                "Boolean" => bool.Parse(configStringValue),
                _ => configStringValue
            };
            propertyInfo.SetValue(appSettings, value);
        }

        private static string GetSettingAsString(IConfigurationRoot configuration, IConfigurationSection section, string settingName, string defaultValue = null)
        {
            var environmentVarSettingsName = (section.Path == DefaultSettingsSection)
                ? settingName.ToUpper()
                : $"{section.Path.ToUpper()}_{settingName.ToUpper()}";
            return Environment.GetEnvironmentVariable(environmentVarSettingsName)
                   ?? section?[settingName]
                   ?? configuration[settingName]
                   ?? defaultValue;
        }
    }
}