using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;


/// <summary>
///  Main class for handling weapon dynamics for all weapons. changing this class directly should be avoided. 
///   Please instead override this class to create different attacks or change functions.
/// </summary>
public partial class WeaponManager : Node2D
{
    private const int _default_weapon = 0;
    protected PlayerManager player;
    protected int weapon_id;
    protected PackedScene projectile = null;

    /// <param name="player">Instance of calling PlayerManager</param>
    /// <param name="weapon_id">Id of the weapon which is referenced within JSON DB</param>
    /// <param name="projectile">Scene with the 
    public WeaponManager(PlayerManager player, int weapon_id = _default_weapon) {
        this.player = player;
        this.weapon_id = weapon_id;
        Item item_model = LoadWeaponData(weapon_id);

        if(item_model != null){
            projectile = ResourceLoader.Load<PackedScene>(item_model.scene);
        }
        else{
            GD.PrintErr($"Weapon with ID {weapon_id} not found.");
        }
    }

    /// <summary>
    /// Loads the weapon.json data file which holds all data for weapons and returns the item we are looking for.
    /// </summary>
    /// <param name="weapon_id">The id of the weapon we are searching for</param>
    /// <returns>Item</returns>
    private Item LoadWeaponData(int weapon_id) {
        string jsonStr = "";
        using (StreamReader r = new StreamReader(@"DataBase\weapons.json")) {
            jsonStr = r.ReadToEnd();
        }
        
        var options = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        };

        var weapon_data = JsonSerializer.Deserialize<Dictionary<string, List<Item>>>(jsonStr, options);
        
        Item default_item = null;

        foreach (var weapon_catergory in weapon_data.Values){
            foreach(var item in weapon_catergory){
                if(item.id == weapon_id){
                    return item;
                }
                else if(item.id == _default_weapon){default_item = item;}
            }
        }
        return default_item;
    }

    public virtual void Attack(){
        GD.Print($"{weapon_id} attack!");

        if(projectile != null){
            Node2D projectileInstance = (Node2D)projectile.Instantiate();
            if(projectileInstance != null){
                // Set the position of the projectile to the player's position
                projectileInstance.GlobalPosition = player.GlobalPosition;

                // Add the projectile instance to the scene tree
                player.GetParent().AddChild(projectileInstance);
            }
            else{GD.PrintErr("Failed to instantiate projectile scene");}
        }
        else{GD.PrintErr("No projectile scene loaded");}
    }


    /// <summary>
    /// Custom item class to map the JSON deserializer data to an object
    /// </summary>
    public class Item{
        public int id { get; set; }
        public string name { get; set; }
        public int damage { get; set; }
        public int range { get; set; }
        public int weight { get; set; }
        public string description { get; set; }
        public string scene { get; set; }
    }

}
