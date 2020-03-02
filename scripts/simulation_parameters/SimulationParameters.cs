﻿using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

public class SimulationParameters
{
    private static readonly SimulationParameters INSTANCE = new SimulationParameters();

    private Dictionary<string, MembraneType> membranes;
    private Dictionary<string, Background> backgrounds;
    private Dictionary<string, Biome> biomes;
    private Dictionary<string, BioProcess> bioProcesses;
    private Dictionary<string, Compound> compounds;
    private Dictionary<string, OrganelleDefinition> organelles;

    static SimulationParameters()
    {
    }

    /// <summary>
    ///   Loads the simulation configuration parameters from JSON files
    /// </summary>
    private SimulationParameters()
    {
        membranes = LoadRegistry<MembraneType>(
            "res://scripts/simulation_parameters/microbe_stage/membranes.json");
        backgrounds = LoadRegistry<Background>(
            "res://scripts/simulation_parameters/microbe_stage/backgrounds.json");
        biomes = LoadRegistry<Biome>(
            "res://scripts/simulation_parameters/microbe_stage/biomes.json");
        bioProcesses = LoadRegistry<BioProcess>(
            "res://scripts/simulation_parameters/microbe_stage/bio_processes.json");
        compounds = LoadRegistry<Compound>(
            "res://scripts/simulation_parameters/microbe_stage/compounds.json");
        organelles = LoadRegistry<OrganelleDefinition>(
                    "res://scripts/simulation_parameters/microbe_stage/organelles.json");

        NameGenerator = LoadDirectObject<NameGenerator>(
            "res://scripts/simulation_parameters/microbe_stage/species_names.json");

        GD.Print("SimulationParameters loading ended");
        CheckForInvalidValues();

        // TODO: there could also be a check for making sure
        // non-existant compounds, processes etc. are not used
        GD.Print("SimulationParameters are good");
    }

    public static SimulationParameters Instance
    {
        get
        {
            return INSTANCE;
        }
    }

    public NameGenerator NameGenerator { get; }

    public OrganelleDefinition GetOrganelleType(string name)
    {
        return organelles[name];
    }

    public MembraneType GetMembrane(string name)
    {
        return membranes[name];
    }

    public Background GetBackground(string name)
    {
        return backgrounds[name];
    }

    public Biome GetBiome(string name)
    {
        return biomes[name];
    }

    public BioProcess GetBioProcess(string name)
    {
        return bioProcesses[name];
    }

    public Compound GetCompound(string name)
    {
        return compounds[name];
    }

    private static string ReadJSONFile(string path)
    {
        using (var file = new File())
        {
            file.Open(path, File.ModeFlags.Read);
            var result = file.GetAsText();

            // This might be completely unnecessary
            file.Close();

            return result;
        }
    }

    private Dictionary<string, T> LoadRegistry<T>(string path)
    {
        var result = JsonConvert.DeserializeObject<Dictionary<string, T>>(ReadJSONFile(path));

        GD.Print($"Loaded registry for {typeof(T)} with {result.Count} items");
        return result;
    }

    private T LoadDirectObject<T>(string path)
    {
        return JsonConvert.DeserializeObject<T>(ReadJSONFile(path));
    }

    private void CheckForInvalidValues()
    {
        CheckRegistryType(membranes);
        CheckRegistryType(backgrounds);
        CheckRegistryType(biomes);
        CheckRegistryType(bioProcesses);
        CheckRegistryType(compounds);
        CheckRegistryType(organelles);

        NameGenerator.Check(string.Empty);
    }

    private void CheckRegistryType<T>(Dictionary<string, T> registry)
        where T : IRegistryType
    {
        foreach (var entry in registry)
        {
            entry.Value.Check(entry.Key);
        }
    }
}