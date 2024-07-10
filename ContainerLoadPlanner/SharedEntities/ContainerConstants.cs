using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharedEntities
{
    public static class ContainerConstants
    {
        private class ContainerConfig
        {
            [JsonProperty("default_capacity")]
            public double DefaultCapacity { get; set; }
            [JsonProperty("max_tolerance")]
            public double MaxTolerance { get; set; }
            [JsonProperty("min_acceptable")]
            public int MinAcceptable { get; set; }
        }

        private class ContainerConfigs
        {
            public Dictionary<string, ContainerConfig> Containers { get; set; }
        }

        private static readonly ContainerConfigs containerConfigs;

        static ContainerConstants()
        {
            try
            {
                string filePath = "container-constants.json";
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"The file '{filePath}' was not found.");
                }

                string jsonString = File.ReadAllText(filePath);
                containerConfigs = JsonConvert.DeserializeObject<ContainerConfigs>(jsonString);

                if (containerConfigs == null || containerConfigs.Containers == null)
                {
                    throw new Exception("Failed to deserialize the JSON file into the expected structure.");
                }
                FORTY_HI_DEFAULT_CAPACITY = containerConfigs.Containers["40HC"].DefaultCapacity;
                FORTY_HI_MIN_ACCEPTABLE_VOLUME = containerConfigs.Containers["40HC"].MinAcceptable;

                FORTY_STD_DEFAULT_CAPACITY = containerConfigs.Containers["40STD"].DefaultCapacity;
                FORTY_STD_MIN_ACCEPTABLE_VOLUME = containerConfigs.Containers["40STD"].MinAcceptable;

                TWENTY_STD_DEFAULT_CAPACITY = containerConfigs.Containers["20STD"].DefaultCapacity;
                TWENTY_STD_MIN_ACCEPTABLE_VOLUME = containerConfigs.Containers["20STD"].MinAcceptable;

                FORTY_HI_TOLERANCE = containerConfigs.Containers["40HC"].MaxTolerance;
                FORTY_STD_TOLERANCE = containerConfigs.Containers["40STD"].MaxTolerance;
                TWENTY_STD_TOLERANCE = containerConfigs.Containers["20STD"].MaxTolerance;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Error parsing the JSON data: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An unexpected error occurred: {e.Message}");
                throw;
            }
        }

        public static readonly double FORTY_HI_DEFAULT_CAPACITY;//= containerConfigs.Containers["40HC"].DefaultCapacity;
        public static readonly double FORTY_HI_MIN_ACCEPTABLE_VOLUME;
        public static readonly double FORTY_STD_DEFAULT_CAPACITY;//= containerConfigs.Containers["40STD"].DefaultCapacity;
        public static readonly double FORTY_STD_MIN_ACCEPTABLE_VOLUME;// = containerConfigs.Containers["40STD"].MinAcceptable;
        public static readonly double TWENTY_STD_DEFAULT_CAPACITY;//= containerConfigs.Containers["20STD"].DefaultCapacity;
        public static readonly double TWENTY_STD_MIN_ACCEPTABLE_VOLUME;//= containerConfigs.Containers["20STD"].MinAcceptable;
        public static readonly double FORTY_HI_TOLERANCE;//= containerConfigs.Containers["40HC"].MaxTolerance;
        public static readonly double FORTY_STD_TOLERANCE;// = containerConfigs.Containers["40STD"].MaxTolerance;
        public static readonly double TWENTY_STD_TOLERANCE;// = containerConfigs.Containers["20STD"].MaxTolerance;
    }
}
