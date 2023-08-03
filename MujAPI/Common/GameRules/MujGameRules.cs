using BattleBitAPI.Common;

namespace MujAPI.Common.GameRules
{
	public class MujGameRules
	{
		public ClassBans classBans;
		public WeaponBans weaponBans;
		public GadgetBans gadgetBans;
		public WearingsBans wearingsBans;

		public MujGameRules() 
		{
			this.classBans = new ClassBans();
			this.weaponBans = new WeaponBans();
			this.gadgetBans = new GadgetBans();
			this.wearingsBans = new WearingsBans();
		}

		//class bans
		public class ClassBans
		{
			private Dictionary<GameRole, bool> mClassBans;

			public ClassBans()
			{
				this.mClassBans = new Dictionary<GameRole, bool>();
			}


			/// <summary>
			/// used to ban classes
			/// </summary>
			/// <param name="gameRole"></param>
			public bool BanClass(GameRole gameRole)
			{
				this.mClassBans[gameRole] = true;
				return true;
			}

			/// <summary>
			/// used to unban classes
			/// </summary>
			/// <param name="gameRole"></param>
			public bool UnBanClass(GameRole gameRole)
			{
				this.mClassBans[gameRole] &= false;
				return true;
			}

			/// <summary>
			/// checks if class is banned
			/// </summary>
			/// <param name="gameRole"></param>
			public bool IsBanned(GameRole gameRole)
			{
				return this.mClassBans[gameRole];
			}

			/// <summary>
			/// returns a dictionary of the the class bans
			/// </summary>
			public Dictionary<GameRole, bool> GetClassBans()
			{
				return this.mClassBans;
			}

		}

		//weapon bans
		public class WeaponBans
		{
			private Dictionary<Weapon, bool> mWeaponBans;

			public WeaponBans()
			{
				this.mWeaponBans = new Dictionary<Weapon, bool>();
			}

			/// <summary>
			/// adds weapon to ban list <br/>
			/// </summary>
			/// 
			/// <remarks>
			/// returns false if its already on ban list<br/>
			/// returns true if its not on ban list
			/// </remarks>
			/// <param name="weaponName"></param>
			public bool BanWeapon(Weapon weapon)
			{
				if (!mWeaponBans.ContainsKey(weapon))
				{
					mWeaponBans[weapon] = true;
					return true;
				}
				else
					return false;
			}

			/// <summary>
			/// removes weapon from ban list
			/// </summary>
			/// 
			/// <remarks>
			/// returns true if in ban list<br/>
			/// returns false if not in ban list
			/// </remarks>
			/// <param name="weaponName"></param>
			public bool UnBanWeapon(Weapon weapon)
			{
				if (mWeaponBans.ContainsKey(weapon))
				{
					mWeaponBans.Remove(weapon);
					return true;
				}
				else
					return false;
			}

			/// <summary>
			/// checks if weapon is banned <br/>
			/// </summary>
			/// <remarks>
			/// true if banned <br/>
			/// false if not banned
			/// </remarks>
			/// <param name="weaponName"></param>
			public bool IsBanned(Weapon weapon)
			{
				return mWeaponBans.ContainsKey(weapon);			
			}

			/// <summary>
			/// returns a Dictionary of the weapons with <br/>
			/// their names and Weapon object
			/// </summary>
			public Dictionary<Weapon, bool> GetBanList()
			{
				return mWeaponBans;
			}

		}
	
		//gadget bans
		public class GadgetBans
		{
			private Dictionary<Gadget, bool> mGadgetBans;

			public GadgetBans()
			{
				this.mGadgetBans = new Dictionary<Gadget, bool>();
			}

			/// <summary>
			/// adds the gadget to the ban list
			/// </summary>
			/// 
			/// <remarks>
			/// true if added to ban list<br/>
			/// false if already in ban list
			/// </remarks> 
			/// <param name="gadget"></param>
			public bool BanGadget(Gadget gadget)
			{
				if (!mGadgetBans.ContainsKey(gadget))
				{
					mGadgetBans[gadget] = true;
					return true;
				}
				else
					return false;
			}

			/// <summary>
			/// removed gadget from ban list<br/>
			/// </summary>
			///
			/// <remarks>
			/// true if removed from ban list<br/>
			/// false if its not in ban list
			/// </remarks>
			/// <param name="gadget"></param>
			public bool UnBanGadget(Gadget gadget)
			{
				if (mGadgetBans.ContainsKey(gadget))
				{
					mGadgetBans.Remove(gadget);
					return true;
				}
				else
					return false;
			}

			/// <summary>
			/// checks if weapon is banned <br/>
			/// <summary/>
			/// 
			/// <remarks>
			/// true if banned <br/>
			/// false if not banned
			/// </remarks>
			/// <param name="gadget"></param>
			public bool IsBanned(Gadget gadget)
			{
				return mGadgetBans.ContainsKey(gadget);
			}

			/// <summary>
			/// returns a dictionary of the gadget ban list
			/// </summary>
			public Dictionary <Gadget, bool> GetBanList()
			{
				return mGadgetBans;
			}
		}

		//wearings bans - TODO: needs work
		public class WearingsBans
		{
			private Dictionary<PlayerWearings, bool> mWearingsBans;

			public WearingsBans()
			{
				this.mWearingsBans = new Dictionary<PlayerWearings, bool>();
			}

			/// <summary>
			/// adds the gadget to the ban list
			/// </summary>
			/// 
			/// <remarks>
			/// true if added to ban list<br/>
			/// false if already in ban list
			/// </remarks> 
			/// <param name="playerWearings"></param>
			public bool BanWearings(PlayerWearings playerWearings)
			{
				if (!mWearingsBans.ContainsKey(playerWearings))
				{
					mWearingsBans[playerWearings] = true;
					return true;
				}
				else
					return false;
			}

			/// <summary>
			/// removed wearing from ban list<br/>
			/// </summary>
			///
			/// <remarks>
			/// true if removed from ban list<br/>
			/// false if its not in ban list
			/// </remarks>
			/// <param name="playerWearings"></param>
			public bool UnBanWearings(PlayerWearings playerWearings)
			{
				if (mWearingsBans.ContainsKey(playerWearings))
				{
					mWearingsBans.Remove(playerWearings);
					return true;
				}
				else
					return false;
			}

			/// <summary>
			/// checks if wearing is banned <br/>
			/// <summary/>
			/// 
			/// <remarks>
			/// true if banned <br/>
			/// false if not banned
			/// </remarks>
			/// <param name="playerWearings"></param>
			public bool IsBanned(PlayerWearings playerWearings)
			{
				// TODO: implement wearings ban check, needs to be able to return the banned clothing
				return false;
			}


			/// <summary>
			/// returns a dictionary of the Wearings ban list
			/// </summary>
			public Dictionary<PlayerWearings, bool> GetBanList()
			{
				return this.mWearingsBans;
			}

		}
	}
}
