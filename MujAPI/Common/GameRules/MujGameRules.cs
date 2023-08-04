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
			private HashSet<GameRole> mClassBans;

			public ClassBans()
			{
				this.mClassBans = new HashSet<GameRole>();
			}


			/// <summary>
			/// used to ban classes
			/// </summary>
			/// <param name="gameRole"></param>
			public bool BanClass(GameRole gameRole)
			{
				return true;
			}

			/// <summary>
			/// used to unban classes
			/// </summary>
			/// <param name="gameRole"></param>
			public bool UnBanClass(GameRole gameRole)
			{
				this.mClassBans.Remove(gameRole);
				return true;
			}

			/// <summary>
			/// checks if class is banned
			/// </summary>
			/// <param name="gameRole"></param>
			public bool IsBanned(GameRole gameRole)
			{
				return this.mClassBans.Contains(gameRole);
			}

			/// <summary>
			/// returns a dictionary of the the class bans
			/// </summary>
			public HashSet<GameRole> GetClassBans()
			{
				return this.mClassBans;
			}

		}

		//weapon bans
		public class WeaponBans
		{
			private HashSet<Weapon> mWeaponBans;

			public WeaponBans()
			{
				this.mWeaponBans = new HashSet<Weapon>();
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
				if (!mWeaponBans.Contains(weapon))
				{
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
				if (mWeaponBans.Contains(weapon))
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
				return mWeaponBans.Contains(weapon);			
			}

			/// <summary>
			/// returns a Dictionary of the weapons with <br/>
			/// their names and Weapon object
			/// </summary>
			public HashSet<Weapon> GetBanList()
			{
				return mWeaponBans;
			}

		}
	
		//gadget bans
		public class GadgetBans
		{
			private HashSet<Gadget> mGadgetBans;

			public GadgetBans()
			{
				this.mGadgetBans = new HashSet<Gadget>();
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
				if (!mGadgetBans.Contains(gadget))
				{
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
				if (mGadgetBans.Contains(gadget))
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
				return mGadgetBans.Contains(gadget);
			}

			/// <summary>
			/// returns a dictionary of the gadget ban list
			/// </summary>
			public HashSet<Gadget> GetBanList()
			{
				return mGadgetBans;
			}
		}

		//wearings bans
		public class WearingsBans
		{
			private HashSet<PlayerWearings> mWearingsBans;

			public WearingsBans()
			{
				this.mWearingsBans = new HashSet<PlayerWearings>();
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
				if (!mWearingsBans.Contains(playerWearings))
				{
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
				if (mWearingsBans.Contains(playerWearings))
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
			public async Task<(bool IsBanned, List<string> BannedItems)> IsBanned(PlayerWearings playerWearings)
			{
				List<string> BannedItems = new List<string>();

				//check if playerWearings contains any banned items - need a more efficient solution 😭😭
				foreach (var BannedWearings in mWearingsBans)
				{
					if (BannedWearings.Equals(playerWearings))
					{
						return await Task.FromResult( ( true, new List<string>() ) );
					}

					if (BannedWearings.Head == playerWearings.Head)
						BannedItems.Add("Head");
					
					if (BannedWearings.Chest == playerWearings.Chest)
						BannedItems.Add("Chest");
					
					if (BannedWearings.Belt == playerWearings.Belt)
						BannedItems.Add("Belt");
					
					if (BannedWearings.Backbag == playerWearings.Backbag)
						BannedItems.Add("Backbag");
					
					if (BannedWearings.Eye == playerWearings.Eye)
						BannedItems.Add("Eye");
					
					if (BannedWearings.Face == playerWearings.Face)
						BannedItems.Add("Face");
					
					if (BannedWearings.Hair == playerWearings.Hair)
						BannedItems.Add("Hair");
					
					if (BannedWearings.Skin == playerWearings.Skin)
						BannedItems.Add("Skin");
					
					if (BannedWearings.Uniform == playerWearings.Uniform)
						BannedItems.Add("Uniform");
					
					if (BannedWearings.Camo == playerWearings.Camo)
						BannedItems.Add("Camo");
				}

				if (BannedItems.Count > 0)
				{
					return await Task.FromResult( (true, BannedItems) );
				}
				else
				{ 
					return await Task.FromResult( ( false, new List<string>() ) ); 
				}
			}


			/// <summary>
			/// returns a dictionary of the Wearings ban list
			/// </summary>
			public HashSet<PlayerWearings> GetBanList()
			{
				return this.mWearingsBans;
			}

		}
	}
}
