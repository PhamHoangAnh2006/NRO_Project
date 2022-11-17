using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public class Controller : IMessageHandler
{
	protected static Controller me;

	protected static Controller me2;

	public Message messWait;

	public static bool isLoadingData;

	public static bool isConnectOK;

	public static bool isConnectionFail;

	public static bool isDisconnected;

	public static bool isMain;

	private float demCount;

	private int move;

	private int total;

	public static bool isStopReadMessage;

	public static MyHashTable frameHT_NEWBOSS = new MyHashTable();

	public static Controller gI()
	{
		if (me == null)
		{
			me = new Controller();
		}
		return me;
	}

	public static Controller gI2()
	{
		if (me2 == null)
		{
			me2 = new Controller();
		}
		return me2;
	}

	public void onConnectOK(bool isMain1)
	{
		isMain = isMain1;
		mSystem.onConnectOK();
	}

	public void onConnectionFail(bool isMain1)
	{
		isMain = isMain1;
		mSystem.onConnectionFail();
	}

	public void onDisconnected(bool isMain1)
	{
		isMain = isMain1;
		mSystem.onDisconnected();
	}

	public void requestItemPlayer(Message msg)
	{
		try
		{
			int num = msg.reader().readUnsignedByte();
			Item item = GameScr.currentCharViewInfo.arrItemBody[num];
			item.saleCoinLock = msg.reader().readInt();
			item.sys = msg.reader().readByte();
			item.options = new MyVector();
			try
			{
				while (true)
				{
					item.options.addElement(new ItemOption(msg.reader().readUnsignedByte(), msg.reader().readUnsignedShort()));
				}
			}
			catch (Exception ex)
			{
				Cout.println("Loi tairequestItemPlayer 1" + ex.ToString());
			}
		}
		catch (Exception ex2)
		{
			Cout.println("Loi tairequestItemPlayer 2" + ex2.ToString());
		}
	}

	public void onMessage(Message msg)
	{
		GameCanvas.debugSession.removeAllElements();
		GameCanvas.debug("SA1", 2);
		try
		{
			Char @char = null;
			Mob mob = null;
			MyVector myVector = new MyVector();
			int num = 0;
			Controller2.readMessage(msg);
			switch (msg.command)
			{
			case 66:
				readGetImgByName(msg);
				break;
			case 65:
			{
				sbyte b53 = msg.reader().readSByte();
				string text6 = msg.reader().readUTF();
				short num140 = msg.reader().readShort();
				if (ItemTime.isExistMessage(b53))
				{
					if (num140 != 0)
					{
						ItemTime.getMessageById(b53).initTimeText(b53, text6, num140);
					}
					else
					{
						GameScr.textTime.removeElement(ItemTime.getMessageById(b53));
					}
				}
				else
				{
					ItemTime itemTime = new ItemTime();
					itemTime.initTimeText(b53, text6, num140);
					GameScr.textTime.addElement(itemTime);
				}
				break;
			}
			case 112:
			{
				sbyte b34 = msg.reader().readByte();
				Res.outz("spec type= " + b34);
				if (b34 == 0)
				{
					Panel.spearcialImage = msg.reader().readShort();
					Panel.specialInfo = msg.reader().readUTF();
				}
				else
				{
					if (b34 != 1)
					{
						break;
					}
					sbyte b35 = msg.reader().readByte();
					Char.myCharz().infoSpeacialSkill = new string[b35][];
					Char.myCharz().imgSpeacialSkill = new short[b35][];
					GameCanvas.panel.speacialTabName = new string[b35][];
					for (int num72 = 0; num72 < b35; num72++)
					{
						GameCanvas.panel.speacialTabName[num72] = new string[2];
						string[] array7 = Res.split(msg.reader().readUTF(), "\n", 0);
						if (array7.Length == 2)
						{
							GameCanvas.panel.speacialTabName[num72] = array7;
						}
						if (array7.Length == 1)
						{
							GameCanvas.panel.speacialTabName[num72][0] = array7[0];
							GameCanvas.panel.speacialTabName[num72][1] = string.Empty;
						}
						int num73 = msg.reader().readByte();
						Char.myCharz().infoSpeacialSkill[num72] = new string[num73];
						Char.myCharz().imgSpeacialSkill[num72] = new short[num73];
						for (int num74 = 0; num74 < num73; num74++)
						{
							Char.myCharz().imgSpeacialSkill[num72][num74] = msg.reader().readShort();
							Char.myCharz().infoSpeacialSkill[num72][num74] = msg.reader().readUTF();
						}
					}
					GameCanvas.panel.tabName[25] = GameCanvas.panel.speacialTabName;
					GameCanvas.panel.setTypeSpeacialSkill();
					GameCanvas.panel.show();
				}
				break;
			}
			case -98:
			{
				sbyte b61 = msg.reader().readByte();
				GameCanvas.menu.showMenu = false;
				if (b61 == 0)
				{
					GameCanvas.startYesNoDlg(msg.reader().readUTF(), new Command(mResources.YES, GameCanvas.instance, 888397, msg.reader().readUTF()), new Command(mResources.NO, GameCanvas.instance, 888396, null));
				}
				break;
			}
			case -97:
				Char.myCharz().cNangdong = msg.reader().readInt();
				break;
			case -96:
			{
				sbyte typeTop = msg.reader().readByte();
				GameCanvas.panel.vTop.removeAllElements();
				string topName = msg.reader().readUTF();
				sbyte b39 = msg.reader().readByte();
				for (int num88 = 0; num88 < b39; num88++)
				{
					int rank = msg.reader().readInt();
					int pId = msg.reader().readInt();
					short headID = msg.reader().readShort();
					short body = msg.reader().readShort();
					short leg = msg.reader().readShort();
					string name = msg.reader().readUTF();
					string info3 = msg.reader().readUTF();
					TopInfo topInfo = new TopInfo();
					topInfo.rank = rank;
					topInfo.headID = headID;
					topInfo.body = body;
					topInfo.leg = leg;
					topInfo.name = name;
					topInfo.info = info3;
					topInfo.info2 = msg.reader().readUTF();
					topInfo.pId = pId;
					GameCanvas.panel.vTop.addElement(topInfo);
				}
				GameCanvas.panel.topName = topName;
				GameCanvas.panel.setTypeTop(typeTop);
				GameCanvas.panel.show();
				break;
			}
			case -94:
				while (msg.reader().available() > 0)
				{
					short num107 = msg.reader().readShort();
					int num108 = msg.reader().readInt();
					for (int num109 = 0; num109 < Char.myCharz().vSkill.size(); num109++)
					{
						Skill skill = (Skill)Char.myCharz().vSkill.elementAt(num109);
						if (skill != null && skill.skillId == num107)
						{
							if (num108 < skill.coolDown)
							{
								skill.lastTimeUseThisSkill = mSystem.currentTimeMillis() - (skill.coolDown - num108);
							}
							Res.outz("1 chieu id= " + skill.template.id + " cooldown= " + num108 + "curr cool down= " + skill.coolDown);
						}
					}
				}
				break;
			case -95:
			{
				sbyte b47 = msg.reader().readByte();
				Res.outz("type= " + b47);
				if (b47 == 0)
				{
					int num117 = msg.reader().readInt();
					short templateId = msg.reader().readShort();
					int num118 = msg.readInt3Byte();
					SoundMn.gI().explode_1();
					if (num117 == Char.myCharz().charID)
					{
						Char.myCharz().mobMe = new Mob(num117, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, templateId, 1, num118, 0, num118, (short)(Char.myCharz().cx + ((Char.myCharz().cdir != 1) ? (-40) : 40)), (short)Char.myCharz().cy, 4, 0);
						Char.myCharz().mobMe.isMobMe = true;
						EffecMn.addEff(new Effect(18, Char.myCharz().mobMe.x, Char.myCharz().mobMe.y, 2, 10, -1));
						Char.myCharz().tMobMeBorn = 30;
						GameScr.vMob.addElement(Char.myCharz().mobMe);
					}
					else
					{
						@char = GameScr.findCharInMap(num117);
						if (@char != null)
						{
							Mob mob4 = new Mob(num117, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, templateId, 1, num118, 0, num118, (short)@char.cx, (short)@char.cy, 4, 0);
							mob4.isMobMe = true;
							@char.mobMe = mob4;
							GameScr.vMob.addElement(@char.mobMe);
						}
						else
						{
							Mob mob5 = GameScr.findMobInMap(num117);
							if (mob5 == null)
							{
								mob5 = new Mob(num117, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, templateId, 1, num118, 0, num118, -100, -100, 4, 0);
								mob5.isMobMe = true;
								GameScr.vMob.addElement(mob5);
							}
						}
					}
				}
				if (b47 == 1)
				{
					int num119 = msg.reader().readInt();
					int mobId = msg.reader().readByte();
					Res.outz("mod attack id= " + num119);
					if (num119 == Char.myCharz().charID)
					{
						if (GameScr.findMobInMap(mobId) != null)
						{
							Char.myCharz().mobMe.attackOtherMob(GameScr.findMobInMap(mobId));
						}
					}
					else
					{
						@char = GameScr.findCharInMap(num119);
						if (@char != null && GameScr.findMobInMap(mobId) != null)
						{
							@char.mobMe.attackOtherMob(GameScr.findMobInMap(mobId));
						}
					}
				}
				if (b47 == 2)
				{
					int num120 = msg.reader().readInt();
					int num121 = msg.reader().readInt();
					int num122 = msg.readInt3Byte();
					int cHPNew = msg.readInt3Byte();
					if (num120 == Char.myCharz().charID)
					{
						Res.outz("mob dame= " + num122);
						@char = GameScr.findCharInMap(num121);
						if (@char != null)
						{
							@char.cHPNew = cHPNew;
							if (Char.myCharz().mobMe.isBusyAttackSomeOne)
							{
								@char.doInjure(num122, 0, isCrit: false, isMob: true);
							}
							else
							{
								Char.myCharz().mobMe.dame = num122;
								Char.myCharz().mobMe.setAttack(@char);
							}
						}
					}
					else
					{
						mob = GameScr.findMobInMap(num120);
						if (mob != null)
						{
							if (num121 == Char.myCharz().charID)
							{
								Char.myCharz().cHPNew = cHPNew;
								if (mob.isBusyAttackSomeOne)
								{
									Char.myCharz().doInjure(num122, 0, isCrit: false, isMob: true);
								}
								else
								{
									mob.dame = num122;
									mob.setAttack(Char.myCharz());
								}
							}
							else
							{
								@char = GameScr.findCharInMap(num121);
								if (@char != null)
								{
									@char.cHPNew = cHPNew;
									if (mob.isBusyAttackSomeOne)
									{
										@char.doInjure(num122, 0, isCrit: false, isMob: true);
									}
									else
									{
										mob.dame = num122;
										mob.setAttack(@char);
									}
								}
							}
						}
					}
				}
				if (b47 == 3)
				{
					int num123 = msg.reader().readInt();
					int mobId2 = msg.reader().readInt();
					int hp = msg.readInt3Byte();
					int num124 = msg.readInt3Byte();
					@char = null;
					@char = ((Char.myCharz().charID != num123) ? GameScr.findCharInMap(num123) : Char.myCharz());
					if (@char != null)
					{
						mob = GameScr.findMobInMap(mobId2);
						if (@char.mobMe != null)
						{
							@char.mobMe.attackOtherMob(mob);
						}
						if (mob != null)
						{
							mob.hp = hp;
							if (num124 == 0)
							{
								mob.x = mob.xFirst;
								mob.y = mob.yFirst;
								GameScr.startFlyText(mResources.miss, mob.x, mob.y - mob.h, 0, -2, mFont.MISS);
							}
							else
							{
								GameScr.startFlyText("-" + num124, mob.x, mob.y - mob.h, 0, -2, mFont.ORANGE);
							}
						}
					}
				}
				if (b47 == 4)
				{
				}
				if (b47 == 5)
				{
					int num125 = msg.reader().readInt();
					sbyte b48 = msg.reader().readByte();
					int mobId3 = msg.reader().readInt();
					int num126 = msg.readInt3Byte();
					int hp2 = msg.readInt3Byte();
					@char = null;
					@char = ((num125 != Char.myCharz().charID) ? GameScr.findCharInMap(num125) : Char.myCharz());
					if (@char == null)
					{
						return;
					}
					if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
					{
						@char.setSkillPaint(GameScr.sks[b48], 0);
					}
					else
					{
						@char.setSkillPaint(GameScr.sks[b48], 1);
					}
					Mob mob6 = GameScr.findMobInMap(mobId3);
					if (@char.cx <= mob6.x)
					{
						@char.cdir = 1;
					}
					else
					{
						@char.cdir = -1;
					}
					@char.mobFocus = mob6;
					mob6.hp = hp2;
					GameCanvas.debug("SA83v2", 2);
					if (num126 == 0)
					{
						mob6.x = mob6.xFirst;
						mob6.y = mob6.yFirst;
						GameScr.startFlyText(mResources.miss, mob6.x, mob6.y - mob6.h, 0, -2, mFont.MISS);
					}
					else
					{
						GameScr.startFlyText("-" + num126, mob6.x, mob6.y - mob6.h, 0, -2, mFont.ORANGE);
					}
				}
				if (b47 == 6)
				{
					int num127 = msg.reader().readInt();
					if (num127 == Char.myCharz().charID)
					{
						Char.myCharz().mobMe.startDie();
					}
					else
					{
						GameScr.findCharInMap(num127)?.mobMe.startDie();
					}
				}
				if (b47 != 7)
				{
					break;
				}
				int num128 = msg.reader().readInt();
				if (num128 == Char.myCharz().charID)
				{
					Char.myCharz().mobMe = null;
					for (int num129 = 0; num129 < GameScr.vMob.size(); num129++)
					{
						if (((Mob)GameScr.vMob.elementAt(num129)).mobId == num128)
						{
							GameScr.vMob.removeElementAt(num129);
						}
					}
					break;
				}
				@char = GameScr.findCharInMap(num128);
				for (int num130 = 0; num130 < GameScr.vMob.size(); num130++)
				{
					if (((Mob)GameScr.vMob.elementAt(num130)).mobId == num128)
					{
						GameScr.vMob.removeElementAt(num130);
					}
				}
				if (@char != null)
				{
					@char.mobMe = null;
				}
				break;
			}
			case -92:
				Main.typeClient = msg.reader().readByte();
				Rms.clearAll();
				Rms.saveRMSInt("clienttype", Main.typeClient);
				Rms.saveRMSInt("lastZoomlevel", mGraphics.zoomLevel);
				GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
				break;
			case -91:
			{
				sbyte b25 = msg.reader().readByte();
				GameCanvas.panel.mapNames = new string[b25];
				GameCanvas.panel.planetNames = new string[b25];
				for (int num52 = 0; num52 < b25; num52++)
				{
					GameCanvas.panel.mapNames[num52] = msg.reader().readUTF();
					GameCanvas.panel.planetNames[num52] = msg.reader().readUTF();
				}
				GameCanvas.panel.setTypeMapTrans();
				GameCanvas.panel.show();
				break;
			}
			case -90:
			{
				sbyte b6 = msg.reader().readByte();
				Res.outz("type = " + b6);
				int num9 = msg.reader().readInt();
				if (b6 != -1)
				{
					short num10 = msg.reader().readShort();
					short num11 = msg.reader().readShort();
					short num12 = msg.reader().readShort();
					sbyte b7 = msg.reader().readByte();
					Res.outz("is Monkey = " + b7);
					if (Char.myCharz().charID == num9)
					{
						Char.myCharz().isMask = true;
						Char.myCharz().isMonkey = b7;
						if (Char.myCharz().isMonkey != 0)
						{
							Char.myCharz().isWaitMonkey = false;
							Char.myCharz().isLockMove = false;
						}
					}
					else if (GameScr.findCharInMap(num9) != null)
					{
						GameScr.findCharInMap(num9).isMask = true;
						GameScr.findCharInMap(num9).isMonkey = b7;
					}
					if (num10 != -1)
					{
						if (num9 == Char.myCharz().charID)
						{
							Char.myCharz().head = num10;
						}
						else if (GameScr.findCharInMap(num9) != null)
						{
							GameScr.findCharInMap(num9).head = num10;
						}
					}
					if (num11 != -1)
					{
						if (num9 == Char.myCharz().charID)
						{
							Char.myCharz().body = num11;
						}
						else if (GameScr.findCharInMap(num9) != null)
						{
							GameScr.findCharInMap(num9).body = num11;
						}
					}
					if (num12 != -1)
					{
						if (num9 == Char.myCharz().charID)
						{
							Char.myCharz().leg = num12;
						}
						else if (GameScr.findCharInMap(num9) != null)
						{
							GameScr.findCharInMap(num9).leg = num12;
						}
					}
				}
				if (b6 == -1)
				{
					if (Char.myCharz().charID == num9)
					{
						Char.myCharz().isMask = false;
						Char.myCharz().isMonkey = 0;
					}
					else if (GameScr.findCharInMap(num9) != null)
					{
						GameScr.findCharInMap(num9).isMask = false;
						GameScr.findCharInMap(num9).isMonkey = 0;
					}
				}
				break;
			}
			case -88:
				GameCanvas.endDlg();
				GameCanvas.serverScreen.switchToMe();
				break;
			case -87:
			{
				Res.outz("GET UPDATE_DATA " + msg.reader().available() + " bytes");
				msg.reader().mark(100000);
				createData(msg.reader(), isSaveRMS: true);
				msg.reader().reset();
				sbyte[] data3 = new sbyte[msg.reader().available()];
				msg.reader().readFully(ref data3);
				sbyte[] data4 = new sbyte[1] { GameScr.vcData };
				Rms.saveRMS("NRdataVersion", data4);
				LoginScr.isUpdateData = false;
				if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
				{
					Res.outz(GameScr.vsData + "," + GameScr.vsMap + "," + GameScr.vsSkill + "," + GameScr.vsItem);
					GameScr.gI().readDart();
					GameScr.gI().readEfect();
					GameScr.gI().readArrow();
					GameScr.gI().readSkill();
					Service.gI().clientOk();
					return;
				}
				break;
			}
			case -86:
			{
				sbyte b20 = msg.reader().readByte();
				Res.outz("server gui ve giao dich action = " + b20);
				if (b20 == 0)
				{
					int playerID = msg.reader().readInt();
					GameScr.gI().giaodich(playerID);
				}
				if (b20 == 1)
				{
					int num42 = msg.reader().readInt();
					Char char6 = GameScr.findCharInMap(num42);
					if (char6 == null)
					{
						return;
					}
					GameCanvas.panel.setTypeGiaoDich(char6);
					GameCanvas.panel.show();
					Service.gI().getPlayerMenu(num42);
				}
				if (b20 == 2)
				{
					sbyte b21 = msg.reader().readByte();
					for (int num43 = 0; num43 < GameCanvas.panel.vMyGD.size(); num43++)
					{
						Item item2 = (Item)GameCanvas.panel.vMyGD.elementAt(num43);
						if (item2.indexUI == b21)
						{
							GameCanvas.panel.vMyGD.removeElement(item2);
							break;
						}
					}
				}
				if (b20 == 5)
				{
				}
				if (b20 == 6)
				{
					GameCanvas.panel.isFriendLock = true;
					if (GameCanvas.panel2 != null)
					{
						GameCanvas.panel2.isFriendLock = true;
					}
					GameCanvas.panel.vFriendGD.removeAllElements();
					if (GameCanvas.panel2 != null)
					{
						GameCanvas.panel2.vFriendGD.removeAllElements();
					}
					int friendMoneyGD = msg.reader().readInt();
					sbyte b22 = msg.reader().readByte();
					Res.outz("item size = " + b22);
					for (int num44 = 0; num44 < b22; num44++)
					{
						Item item3 = new Item();
						item3.template = ItemTemplates.get(msg.reader().readShort());
						item3.quantity = msg.reader().readByte();
						int num45 = msg.reader().readUnsignedByte();
						if (num45 != 0)
						{
							item3.itemOption = new ItemOption[num45];
							for (int num46 = 0; num46 < item3.itemOption.Length; num46++)
							{
								int num47 = msg.reader().readUnsignedByte();
								int param3 = msg.reader().readUnsignedShort();
								if (num47 != -1)
								{
									item3.itemOption[num46] = new ItemOption(num47, param3);
									item3.compare = GameCanvas.panel.getCompare(item3);
								}
							}
						}
						if (GameCanvas.panel2 != null)
						{
							GameCanvas.panel2.vFriendGD.addElement(item3);
						}
						else
						{
							GameCanvas.panel.vFriendGD.addElement(item3);
						}
					}
					if (GameCanvas.panel2 != null)
					{
						GameCanvas.panel2.setTabGiaoDich(isMe: false);
						GameCanvas.panel2.friendMoneyGD = friendMoneyGD;
					}
					else
					{
						GameCanvas.panel.friendMoneyGD = friendMoneyGD;
						if (GameCanvas.panel.currentTabIndex == 2)
						{
							GameCanvas.panel.setTabGiaoDich(isMe: false);
						}
					}
				}
				if (b20 == 7)
				{
					InfoDlg.hide();
					if (GameCanvas.panel.isShow)
					{
						GameCanvas.panel.hide();
					}
				}
				break;
			}
			case -85:
			{
				Res.outz("CAP CHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
				sbyte b49 = msg.reader().readByte();
				if (b49 == 0)
				{
					int num131 = msg.reader().readUnsignedShort();
					Res.outz("lent =" + num131);
					sbyte[] data5 = new sbyte[num131];
					msg.reader().read(ref data5, 0, num131);
					GameScr.imgCapcha = Image.createImage(data5, 0, num131);
					GameScr.gI().keyInput = "-----";
					GameScr.gI().strCapcha = msg.reader().readUTF();
					GameScr.gI().keyCapcha = new int[GameScr.gI().strCapcha.Length];
					GameScr.gI().mobCapcha = new Mob();
					GameScr.gI().right = null;
				}
				if (b49 == 1)
				{
					MobCapcha.isAttack = true;
				}
				if (b49 == 2)
				{
					MobCapcha.explode = true;
					GameScr.gI().right = GameScr.gI().cmdFocus;
				}
				break;
			}
			case -112:
			{
				sbyte b36 = msg.reader().readByte();
				if (b36 == 0)
				{
					sbyte mobIndex = msg.reader().readByte();
					GameScr.findMobInMap(mobIndex).clearBody();
				}
				if (b36 == 1)
				{
					sbyte mobIndex2 = msg.reader().readByte();
					GameScr.findMobInMap(mobIndex2).setBody(msg.reader().readShort());
				}
				break;
			}
			case -84:
			{
				int index2 = msg.reader().readUnsignedByte();
				Mob mob8 = null;
				try
				{
					mob8 = (Mob)GameScr.vMob.elementAt(index2);
				}
				catch (Exception)
				{
				}
				if (mob8 != null)
				{
					mob8.maxHp = msg.reader().readInt();
				}
				break;
			}
			case -83:
			{
				sbyte b45 = msg.reader().readByte();
				if (b45 == 0)
				{
					int num110 = msg.reader().readShort();
					int bgRID = msg.reader().readShort();
					int num111 = msg.reader().readUnsignedByte();
					int num112 = msg.reader().readInt();
					string text3 = msg.reader().readUTF();
					int num113 = msg.reader().readShort();
					int num114 = msg.reader().readShort();
					sbyte b46 = msg.reader().readByte();
					if (b46 == 1)
					{
						GameScr.gI().isRongNamek = true;
					}
					else
					{
						GameScr.gI().isRongNamek = false;
					}
					GameScr.gI().xR = num113;
					GameScr.gI().yR = num114;
					Res.outz("xR= " + num113 + " yR= " + num114 + " +++++++++++++++++++++++++++++++++++++++");
					if (Char.myCharz().charID == num112)
					{
						GameCanvas.panel.hideNow();
						GameScr.gI().activeRongThanEff(isMe: true);
					}
					else if (TileMap.mapID == num110 && TileMap.zoneID == num111)
					{
						GameScr.gI().activeRongThanEff(isMe: false);
					}
					else if (mGraphics.zoomLevel > 1)
					{
						GameScr.gI().doiMauTroi();
					}
					GameScr.gI().mapRID = num110;
					GameScr.gI().bgRID = bgRID;
					GameScr.gI().zoneRID = num111;
				}
				if (b45 == 1)
				{
					Res.outz("map RID = " + GameScr.gI().mapRID + " zone RID= " + GameScr.gI().zoneRID);
					Res.outz("map ID = " + TileMap.mapID + " zone ID= " + TileMap.zoneID);
					if (TileMap.mapID == GameScr.gI().mapRID && TileMap.zoneID == GameScr.gI().zoneRID)
					{
						GameScr.gI().hideRongThanEff();
					}
					else
					{
						GameScr.gI().isRongThanXuatHien = false;
						if (GameScr.gI().isRongNamek)
						{
							GameScr.gI().isRongNamek = false;
						}
					}
				}
				if (b45 != 2)
				{
				}
				break;
			}
			case -82:
			{
				sbyte b31 = msg.reader().readByte();
				TileMap.tileIndex = new int[b31][][];
				TileMap.tileType = new int[b31][];
				for (int num69 = 0; num69 < b31; num69++)
				{
					sbyte b32 = msg.reader().readByte();
					TileMap.tileType[num69] = new int[b32];
					TileMap.tileIndex[num69] = new int[b32][];
					for (int num70 = 0; num70 < b32; num70++)
					{
						TileMap.tileType[num69][num70] = msg.reader().readInt();
						sbyte b33 = msg.reader().readByte();
						TileMap.tileIndex[num69][num70] = new int[b33];
						for (int num71 = 0; num71 < b33; num71++)
						{
							TileMap.tileIndex[num69][num70][num71] = msg.reader().readByte();
						}
					}
				}
				break;
			}
			case -81:
			{
				sbyte b17 = msg.reader().readByte();
				if (b17 == 0)
				{
					string src = msg.reader().readUTF();
					string src2 = msg.reader().readUTF();
					GameCanvas.panel.setTypeCombine();
					GameCanvas.panel.combineInfo = mFont.tahoma_7b_blue.splitFontArray(src, Panel.WIDTH_PANEL);
					GameCanvas.panel.combineTopInfo = mFont.tahoma_7.splitFontArray(src2, Panel.WIDTH_PANEL);
					GameCanvas.panel.show();
				}
				if (b17 == 1)
				{
					GameCanvas.panel.vItemCombine.removeAllElements();
					sbyte b18 = msg.reader().readByte();
					for (int num38 = 0; num38 < b18; num38++)
					{
						sbyte b19 = msg.reader().readByte();
						for (int num39 = 0; num39 < Char.myCharz().arrItemBag.Length; num39++)
						{
							Item item = Char.myCharz().arrItemBag[num39];
							if (item != null && item.indexUI == b19)
							{
								item.isSelect = true;
								GameCanvas.panel.vItemCombine.addElement(item);
							}
						}
					}
					if (GameCanvas.panel.isShow)
					{
						GameCanvas.panel.setTabCombine();
					}
				}
				if (b17 > 1)
				{
					int num40 = 21;
					for (int num41 = 0; num41 < GameScr.vNpc.size(); num41++)
					{
						Npc npc = (Npc)GameScr.vNpc.elementAt(num41);
						if (npc.template.npcTemplateId == num40)
						{
							GameCanvas.panel.xS = npc.cx - GameScr.cmx;
							GameCanvas.panel.yS = npc.cy - GameScr.cmy;
							GameCanvas.panel.idNPC = num40;
							break;
						}
					}
				}
				if (b17 == 2)
				{
					GameCanvas.panel.combineSuccess = 0;
					GameCanvas.panel.setCombineEff(0);
				}
				if (b17 == 3)
				{
					GameCanvas.panel.combineSuccess = 1;
					GameCanvas.panel.setCombineEff(0);
				}
				if (b17 == 4)
				{
					short iconID = msg.reader().readShort();
					GameCanvas.panel.iconID3 = iconID;
					GameCanvas.panel.combineSuccess = 0;
					GameCanvas.panel.setCombineEff(1);
				}
				if (b17 == 5)
				{
					short iconID2 = msg.reader().readShort();
					GameCanvas.panel.iconID3 = iconID2;
					GameCanvas.panel.combineSuccess = 0;
					GameCanvas.panel.setCombineEff(2);
				}
				if (b17 == 6)
				{
					short iconID3 = msg.reader().readShort();
					short iconID4 = msg.reader().readShort();
					GameCanvas.panel.combineSuccess = 0;
					GameCanvas.panel.setCombineEff(3);
					GameCanvas.panel.iconID1 = iconID3;
					GameCanvas.panel.iconID3 = iconID4;
				}
				break;
			}
			case -80:
			{
				sbyte b38 = msg.reader().readByte();
				InfoDlg.hide();
				if (b38 == 0)
				{
					GameCanvas.panel.vFriend.removeAllElements();
					int num82 = msg.reader().readUnsignedByte();
					for (int num83 = 0; num83 < num82; num83++)
					{
						Char char10 = new Char();
						char10.charID = msg.reader().readInt();
						char10.head = msg.reader().readShort();
						char10.body = msg.reader().readShort();
						char10.leg = msg.reader().readShort();
						char10.bag = msg.reader().readUnsignedByte();
						char10.cName = msg.reader().readUTF();
						bool isOnline = msg.reader().readBoolean();
						InfoItem infoItem2 = new InfoItem(mResources.power + ": " + msg.reader().readUTF());
						infoItem2.charInfo = char10;
						infoItem2.isOnline = isOnline;
						GameCanvas.panel.vFriend.addElement(infoItem2);
					}
					GameCanvas.panel.setTypeFriend();
					GameCanvas.panel.show();
				}
				if (b38 == 3)
				{
					MyVector vFriend = GameCanvas.panel.vFriend;
					int num84 = msg.reader().readInt();
					Res.outz("online offline id=" + num84);
					for (int num85 = 0; num85 < vFriend.size(); num85++)
					{
						InfoItem infoItem3 = (InfoItem)vFriend.elementAt(num85);
						if (infoItem3.charInfo != null && infoItem3.charInfo.charID == num84)
						{
							Res.outz("online= " + infoItem3.isOnline);
							infoItem3.isOnline = msg.reader().readBoolean();
							break;
						}
					}
				}
				if (b38 != 2)
				{
					break;
				}
				MyVector vFriend2 = GameCanvas.panel.vFriend;
				int num86 = msg.reader().readInt();
				for (int num87 = 0; num87 < vFriend2.size(); num87++)
				{
					InfoItem infoItem4 = (InfoItem)vFriend2.elementAt(num87);
					if (infoItem4.charInfo != null && infoItem4.charInfo.charID == num86)
					{
						vFriend2.removeElement(infoItem4);
						break;
					}
				}
				if (GameCanvas.panel.isShow)
				{
					GameCanvas.panel.setTabFriend();
				}
				break;
			}
			case -99:
			{
				InfoDlg.hide();
				sbyte b29 = msg.reader().readByte();
				if (b29 == 0)
				{
					GameCanvas.panel.vEnemy.removeAllElements();
					int num66 = msg.reader().readUnsignedByte();
					for (int num67 = 0; num67 < num66; num67++)
					{
						Char char8 = new Char();
						char8.charID = msg.reader().readInt();
						char8.head = msg.reader().readShort();
						char8.body = msg.reader().readShort();
						char8.leg = msg.reader().readShort();
						char8.bag = msg.reader().readShort();
						char8.cName = msg.reader().readUTF();
						InfoItem infoItem = new InfoItem(msg.reader().readUTF());
						bool flag6 = msg.reader().readBoolean();
						infoItem.charInfo = char8;
						infoItem.isOnline = flag6;
						Res.outz("isonline = " + flag6);
						GameCanvas.panel.vEnemy.addElement(infoItem);
					}
					GameCanvas.panel.setTypeEnemy();
					GameCanvas.panel.show();
				}
				break;
			}
			case -79:
			{
				InfoDlg.hide();
				int num90 = msg.reader().readInt();
				Char charMenu = GameCanvas.panel.charMenu;
				if (charMenu == null)
				{
					return;
				}
				charMenu.cPower = msg.reader().readLong();
				charMenu.currStrLevel = msg.reader().readUTF();
				break;
			}
			case -93:
			{
				short num115 = msg.reader().readShort();
				BgItem.newSmallVersion = new sbyte[num115];
				for (int num116 = 0; num116 < num115; num116++)
				{
					BgItem.newSmallVersion[num116] = msg.reader().readByte();
				}
				break;
			}
			case -77:
			{
				short num49 = msg.reader().readShort();
				SmallImage.newSmallVersion = new sbyte[num49];
				SmallImage.maxSmall = num49;
				SmallImage.imgNew = new Small[num49];
				for (int num50 = 0; num50 < num49; num50++)
				{
					SmallImage.newSmallVersion[num50] = msg.reader().readByte();
				}
				break;
			}
			case -76:
			{
				sbyte b58 = msg.reader().readByte();
				if (b58 == 0)
				{
					sbyte b59 = msg.reader().readByte();
					if (b59 <= 0)
					{
						return;
					}
					Char.myCharz().arrArchive = new Archivement[b59];
					for (int num148 = 0; num148 < b59; num148++)
					{
						Char.myCharz().arrArchive[num148] = new Archivement();
						Char.myCharz().arrArchive[num148].info1 = num148 + 1 + ". " + msg.reader().readUTF();
						Char.myCharz().arrArchive[num148].info2 = msg.reader().readUTF();
						Char.myCharz().arrArchive[num148].money = msg.reader().readShort();
						Char.myCharz().arrArchive[num148].isFinish = msg.reader().readBoolean();
						Char.myCharz().arrArchive[num148].isRecieve = msg.reader().readBoolean();
					}
					GameCanvas.panel.setTypeArchivement();
					GameCanvas.panel.show();
				}
				else if (b58 == 1)
				{
					int num149 = msg.reader().readUnsignedByte();
					if (Char.myCharz().arrArchive[num149] != null)
					{
						Char.myCharz().arrArchive[num149].isRecieve = true;
					}
				}
				break;
			}
			case -74:
			{
				if (ServerListScreen.stopDownload)
				{
					return;
				}
				if (!GameCanvas.isGetResourceFromServer())
				{
					Service.gI().getResource(3, null);
					SmallImage.loadBigRMS();
					SplashScr.imgLogo = null;
					if (Rms.loadRMSString("acc") != null || Rms.loadRMSString("userAo" + ServerListScreen.ipSelect) != null)
					{
						LoginScr.isContinueToLogin = true;
					}
					GameCanvas.loginScr = new LoginScr();
					GameCanvas.loginScr.switchToMe();
					return;
				}
				bool flag3 = true;
				sbyte b11 = msg.reader().readByte();
				Res.outz("action = " + b11);
				if (b11 == 0)
				{
					int num21 = msg.reader().readInt();
					string text = Rms.loadRMSString("ResVersion");
					int num22 = ((text == null || !(text != string.Empty)) ? (-1) : int.Parse(text));
					if (num22 == -1 || num22 != num21)
					{
						ServerListScreen.loadScreen = false;
						GameCanvas.serverScreen.show2();
					}
					else
					{
						Res.outz("login ngay");
						SmallImage.loadBigRMS();
						SplashScr.imgLogo = null;
						ServerListScreen.loadScreen = true;
						if (GameCanvas.currentScreen != GameCanvas.loginScr)
						{
							GameCanvas.serverScreen.switchToMe();
						}
					}
				}
				if (b11 == 1)
				{
					ServerListScreen.strWait = mResources.downloading_data;
					short num23 = (short)(ServerListScreen.nBig = msg.reader().readShort());
					Service.gI().getResource(2, null);
				}
				if (b11 == 2)
				{
					try
					{
						isLoadingData = true;
						GameCanvas.endDlg();
						ServerListScreen.demPercent++;
						ServerListScreen.percent = ServerListScreen.demPercent * 100 / ServerListScreen.nBig;
						string original = msg.reader().readUTF();
						string[] array3 = Res.split(original, "/", 0);
						string filename = "x" + mGraphics.zoomLevel + array3[array3.Length - 1];
						int num24 = msg.reader().readInt();
						sbyte[] data2 = new sbyte[num24];
						msg.reader().read(ref data2, 0, num24);
						Rms.saveRMS(filename, data2);
					}
					catch (Exception)
					{
						GameCanvas.startOK(mResources.pls_restart_game_error, 8885, null);
					}
				}
				if (b11 == 3 && flag3)
				{
					isLoadingData = false;
					int num25 = msg.reader().readInt();
					Res.outz("last version= " + num25);
					Rms.saveRMSString("ResVersion", num25 + string.Empty);
					Service.gI().getResource(3, null);
					GameCanvas.endDlg();
					SplashScr.imgLogo = null;
					SmallImage.loadBigRMS();
					mSystem.gcc();
					ServerListScreen.bigOk = true;
					ServerListScreen.loadScreen = true;
					GameScr.gI().loadGameScr();
					if (GameCanvas.currentScreen != GameCanvas.loginScr)
					{
						GameCanvas.serverScreen.switchToMe();
					}
				}
				break;
			}
			case -43:
			{
				sbyte itemAction = msg.reader().readByte();
				sbyte where = msg.reader().readByte();
				sbyte index = msg.reader().readByte();
				string info2 = msg.reader().readUTF();
				GameCanvas.panel.itemRequest(itemAction, info2, where, index);
				break;
			}
			case -59:
			{
				sbyte typePK = msg.reader().readByte();
				GameScr.gI().player_vs_player(msg.reader().readInt(), msg.reader().readInt(), msg.reader().readUTF(), typePK);
				break;
			}
			case -62:
			{
				int num158 = msg.reader().readUnsignedByte();
				sbyte b68 = msg.reader().readByte();
				if (b68 <= 0)
				{
					break;
				}
				ClanImage clanImage2 = ClanImage.getClanImage((sbyte)num158);
				if (clanImage2 == null)
				{
					break;
				}
				clanImage2.idImage = new short[b68];
				for (int num159 = 0; num159 < b68; num159++)
				{
					clanImage2.idImage[num159] = msg.reader().readShort();
					if (clanImage2.idImage[num159] > 0)
					{
						SmallImage.vKeys.addElement(clanImage2.idImage[num159] + string.Empty);
					}
				}
				break;
			}
			case -65:
			{
				Res.outz("TELEPORT ...................................................");
				InfoDlg.hide();
				int num68 = msg.reader().readInt();
				sbyte b30 = msg.reader().readByte();
				if (b30 == 0)
				{
					break;
				}
				if (Char.myCharz().charID == num68)
				{
					isStopReadMessage = true;
					GameScr.lockTick = 500;
					GameScr.gI().center = null;
					if (b30 == 0 || b30 == 1 || b30 == 3)
					{
						Teleport p = new Teleport(Char.myCharz().cx, Char.myCharz().cy, Char.myCharz().head, Char.myCharz().cdir, 0, isMe: true, (b30 != 1) ? b30 : Char.myCharz().cgender);
						Teleport.addTeleport(p);
					}
					if (b30 == 2)
					{
						GameScr.lockTick = 50;
						Char.myCharz().hide();
					}
				}
				else
				{
					Char char9 = GameScr.findCharInMap(num68);
					if ((b30 == 0 || b30 == 1 || b30 == 3) && char9 != null)
					{
						char9.isUsePlane = true;
						Teleport teleport = new Teleport(char9.cx, char9.cy, char9.head, char9.cdir, 0, isMe: false, (b30 != 1) ? b30 : char9.cgender);
						teleport.id = num68;
						Teleport.addTeleport(teleport);
					}
					if (b30 == 2)
					{
						char9.hide();
					}
				}
				break;
			}
			case -64:
			{
				int num20 = msg.reader().readInt();
				int bag = msg.reader().readUnsignedByte();
				if (num20 == Char.myCharz().charID)
				{
					Char.myCharz().bag = bag;
				}
				else if (GameScr.findCharInMap(num20) != null)
				{
					GameScr.findCharInMap(num20).bag = bag;
				}
				break;
			}
			case -63:
			{
				Res.outz("GET BAG");
				int num164 = msg.reader().readUnsignedByte();
				sbyte b69 = msg.reader().readByte();
				ClanImage clanImage3 = new ClanImage();
				clanImage3.ID = num164;
				if (b69 > 0)
				{
					clanImage3.idImage = new short[b69];
					for (int num165 = 0; num165 < b69; num165++)
					{
						clanImage3.idImage[num165] = msg.reader().readShort();
						Res.outz("ID=  " + num164 + " frame= " + clanImage3.idImage[num165]);
					}
					ClanImage.idImages.put(num164 + string.Empty, clanImage3);
				}
				break;
			}
			case -57:
			{
				string strInvite = msg.reader().readUTF();
				int clanID = msg.reader().readInt();
				int code = msg.reader().readInt();
				GameScr.gI().clanInvite(strInvite, clanID, code);
				break;
			}
			case -51:
				InfoDlg.hide();
				readClanMsg(msg, 0);
				if (GameCanvas.panel.isMessage && GameCanvas.panel.type == 5)
				{
					GameCanvas.panel.initTabClans();
				}
				break;
			case -53:
			{
				Res.outz("MY CLAN INFO");
				InfoDlg.hide();
				bool flag7 = false;
				int num75 = msg.reader().readInt();
				Res.outz("clanId= " + num75);
				if (num75 == -1)
				{
					flag7 = true;
					Char.myCharz().clan = null;
					ClanMessage.vMessage.removeAllElements();
					if (GameCanvas.panel.member != null)
					{
						GameCanvas.panel.member.removeAllElements();
					}
					if (GameCanvas.panel.myMember != null)
					{
						GameCanvas.panel.myMember.removeAllElements();
					}
					if (GameCanvas.currentScreen == GameScr.gI())
					{
						GameCanvas.panel.setTabClans();
					}
					return;
				}
				GameCanvas.panel.tabIcon = null;
				if (Char.myCharz().clan == null)
				{
					Char.myCharz().clan = new Clan();
				}
				Char.myCharz().clan.ID = num75;
				Char.myCharz().clan.name = msg.reader().readUTF();
				Char.myCharz().clan.slogan = msg.reader().readUTF();
				Char.myCharz().clan.imgID = msg.reader().readUnsignedByte();
				Char.myCharz().clan.powerPoint = msg.reader().readUTF();
				Char.myCharz().clan.leaderName = msg.reader().readUTF();
				Char.myCharz().clan.currMember = msg.reader().readUnsignedByte();
				Char.myCharz().clan.maxMember = msg.reader().readUnsignedByte();
				Char.myCharz().role = msg.reader().readByte();
				Char.myCharz().clan.clanPoint = msg.reader().readInt();
				Char.myCharz().clan.level = msg.reader().readByte();
				GameCanvas.panel.myMember = new MyVector();
				for (int num76 = 0; num76 < Char.myCharz().clan.currMember; num76++)
				{
					Member member5 = new Member();
					member5.ID = msg.reader().readInt();
					member5.head = msg.reader().readShort();
					member5.leg = msg.reader().readShort();
					member5.body = msg.reader().readShort();
					member5.name = msg.reader().readUTF();
					member5.role = msg.reader().readByte();
					member5.powerPoint = msg.reader().readUTF();
					member5.donate = msg.reader().readInt();
					member5.receive_donate = msg.reader().readInt();
					member5.clanPoint = msg.reader().readInt();
					member5.curClanPoint = msg.reader().readInt();
					member5.joinTime = NinjaUtil.getDate(msg.reader().readInt());
					GameCanvas.panel.myMember.addElement(member5);
				}
				int num77 = msg.reader().readUnsignedByte();
				for (int num78 = 0; num78 < num77; num78++)
				{
					readClanMsg(msg, -1);
				}
				if (GameCanvas.panel.isSearchClan || GameCanvas.panel.isViewMember || GameCanvas.panel.isMessage)
				{
					GameCanvas.panel.setTabClans();
				}
				if (flag7)
				{
					GameCanvas.panel.setTabClans();
				}
				break;
			}
			case -52:
			{
				sbyte b15 = msg.reader().readByte();
				if (b15 == 0)
				{
					Member member2 = new Member();
					member2.ID = msg.reader().readInt();
					member2.head = msg.reader().readShort();
					member2.leg = msg.reader().readShort();
					member2.body = msg.reader().readShort();
					member2.name = msg.reader().readUTF();
					member2.role = msg.reader().readByte();
					member2.powerPoint = msg.reader().readUTF();
					member2.donate = msg.reader().readInt();
					member2.receive_donate = msg.reader().readInt();
					member2.clanPoint = msg.reader().readInt();
					member2.joinTime = NinjaUtil.getDate(msg.reader().readInt());
					if (GameCanvas.panel.myMember == null)
					{
						GameCanvas.panel.myMember = new MyVector();
					}
					GameCanvas.panel.myMember.addElement(member2);
					GameCanvas.panel.initTabClans();
				}
				if (b15 == 1)
				{
					GameCanvas.panel.myMember.removeElementAt(msg.reader().readByte());
					GameCanvas.panel.currentListLength--;
					GameCanvas.panel.initTabClans();
				}
				if (b15 != 2)
				{
					break;
				}
				Member member3 = new Member();
				member3.ID = msg.reader().readInt();
				member3.head = msg.reader().readShort();
				member3.leg = msg.reader().readShort();
				member3.body = msg.reader().readShort();
				member3.name = msg.reader().readUTF();
				member3.role = msg.reader().readByte();
				member3.powerPoint = msg.reader().readUTF();
				member3.donate = msg.reader().readInt();
				member3.receive_donate = msg.reader().readInt();
				member3.clanPoint = msg.reader().readInt();
				member3.joinTime = NinjaUtil.getDate(msg.reader().readInt());
				for (int n = 0; n < GameCanvas.panel.myMember.size(); n++)
				{
					Member member4 = (Member)GameCanvas.panel.myMember.elementAt(n);
					if (member4.ID == member3.ID)
					{
						if (Char.myCharz().charID == member3.ID)
						{
							Char.myCharz().role = member3.role;
						}
						Member o = member3;
						GameCanvas.panel.myMember.removeElement(member4);
						GameCanvas.panel.myMember.insertElementAt(o, n);
						return;
					}
				}
				break;
			}
			case -50:
			{
				InfoDlg.hide();
				GameCanvas.panel.member = new MyVector();
				sbyte b12 = msg.reader().readByte();
				for (int k = 0; k < b12; k++)
				{
					Member member = new Member();
					member.ID = msg.reader().readInt();
					member.head = msg.reader().readShort();
					member.leg = msg.reader().readShort();
					member.body = msg.reader().readShort();
					member.name = msg.reader().readUTF();
					member.role = msg.reader().readByte();
					member.powerPoint = msg.reader().readUTF();
					member.donate = msg.reader().readInt();
					member.receive_donate = msg.reader().readInt();
					member.clanPoint = msg.reader().readInt();
					member.joinTime = NinjaUtil.getDate(msg.reader().readInt());
					GameCanvas.panel.member.addElement(member);
				}
				GameCanvas.panel.isViewMember = true;
				GameCanvas.panel.isSearchClan = false;
				GameCanvas.panel.isMessage = false;
				GameCanvas.panel.currentListLength = GameCanvas.panel.member.size() + 2;
				GameCanvas.panel.initTabClans();
				break;
			}
			case -47:
			{
				InfoDlg.hide();
				sbyte b13 = msg.reader().readByte();
				Res.outz("clan = " + b13);
				if (b13 == 0)
				{
					GameCanvas.panel.clanReport = mResources.cannot_find_clan;
					GameCanvas.panel.clans = null;
				}
				else
				{
					GameCanvas.panel.clans = new Clan[b13];
					Res.outz("clan search lent= " + GameCanvas.panel.clans.Length);
					for (int l = 0; l < GameCanvas.panel.clans.Length; l++)
					{
						GameCanvas.panel.clans[l] = new Clan();
						GameCanvas.panel.clans[l].ID = msg.reader().readInt();
						GameCanvas.panel.clans[l].name = msg.reader().readUTF();
						GameCanvas.panel.clans[l].slogan = msg.reader().readUTF();
						GameCanvas.panel.clans[l].imgID = msg.reader().readUnsignedByte();
						GameCanvas.panel.clans[l].powerPoint = msg.reader().readUTF();
						GameCanvas.panel.clans[l].leaderName = msg.reader().readUTF();
						GameCanvas.panel.clans[l].currMember = msg.reader().readUnsignedByte();
						GameCanvas.panel.clans[l].maxMember = msg.reader().readUnsignedByte();
						GameCanvas.panel.clans[l].date = msg.reader().readInt();
					}
				}
				GameCanvas.panel.isSearchClan = true;
				GameCanvas.panel.isViewMember = false;
				GameCanvas.panel.isMessage = false;
				if (GameCanvas.panel.isSearchClan)
				{
					GameCanvas.panel.initTabClans();
				}
				break;
			}
			case -46:
			{
				InfoDlg.hide();
				sbyte b51 = msg.reader().readByte();
				if (b51 == 1 || b51 == 3)
				{
					GameCanvas.endDlg();
					ClanImage.vClanImage.removeAllElements();
					int num135 = msg.reader().readUnsignedByte();
					for (int num136 = 0; num136 < num135; num136++)
					{
						ClanImage clanImage = new ClanImage();
						clanImage.ID = msg.reader().readUnsignedByte();
						clanImage.name = msg.reader().readUTF();
						clanImage.xu = msg.reader().readInt();
						clanImage.luong = msg.reader().readInt();
						if (!ClanImage.isExistClanImage(clanImage.ID))
						{
							ClanImage.addClanImage(clanImage);
							continue;
						}
						ClanImage.getClanImage((sbyte)clanImage.ID).name = clanImage.name;
						ClanImage.getClanImage((sbyte)clanImage.ID).xu = clanImage.xu;
						ClanImage.getClanImage((sbyte)clanImage.ID).luong = clanImage.luong;
					}
					if (Char.myCharz().clan != null)
					{
						GameCanvas.panel.changeIcon();
					}
				}
				if (b51 == 4)
				{
					Char.myCharz().clan.imgID = msg.reader().readUnsignedByte();
					Char.myCharz().clan.slogan = msg.reader().readUTF();
				}
				break;
			}
			case -61:
			{
				int num133 = msg.reader().readInt();
				if (num133 != Char.myCharz().charID)
				{
					if (GameScr.findCharInMap(num133) != null)
					{
						GameScr.findCharInMap(num133).clanID = msg.reader().readInt();
						if (GameScr.findCharInMap(num133).clanID == -2)
						{
							GameScr.findCharInMap(num133).isCopy = true;
						}
					}
				}
				else if (Char.myCharz().clan != null)
				{
					Char.myCharz().clan.ID = msg.reader().readInt();
				}
				break;
			}
			case -42:
				Char.myCharz().cHPGoc = msg.readInt3Byte();
				Char.myCharz().cMPGoc = msg.readInt3Byte();
				Char.myCharz().cDamGoc = msg.reader().readInt();
				Char.myCharz().cHPFull = msg.readInt3Byte();
				Char.myCharz().cMPFull = msg.readInt3Byte();
				Char.myCharz().cHP = msg.readInt3Byte();
				Char.myCharz().cMP = msg.readInt3Byte();
				Char.myCharz().cspeed = msg.reader().readByte();
				Char.myCharz().hpFrom1000TiemNang = msg.reader().readByte();
				Char.myCharz().mpFrom1000TiemNang = msg.reader().readByte();
				Char.myCharz().damFrom1000TiemNang = msg.reader().readByte();
				Char.myCharz().cDamFull = msg.reader().readInt();
				Char.myCharz().cDefull = msg.reader().readInt();
				Char.myCharz().cCriticalFull = msg.reader().readByte();
				Char.myCharz().cTiemNang = msg.reader().readLong();
				Char.myCharz().expForOneAdd = msg.reader().readShort();
				Char.myCharz().cDefGoc = msg.reader().readShort();
				Char.myCharz().cCriticalGoc = msg.reader().readByte();
				InfoDlg.hide();
				break;
			case 1:
			{
				bool flag9 = msg.reader().readBool();
				Res.outz("isRes= " + flag9);
				if (!flag9)
				{
					GameCanvas.startOKDlg(msg.reader().readUTF());
					break;
				}
				GameCanvas.loginScr.isLogin2 = false;
				Rms.saveRMSString("userAo" + ServerListScreen.ipSelect, string.Empty);
				GameCanvas.endDlg();
				GameCanvas.loginScr.doLogin();
				break;
			}
			case 2:
				Char.isLoadingMap = true;
				LoginScr.isLoggingIn = false;
				if (!GameScr.isLoadAllData)
				{
					GameScr.gI().initSelectChar();
				}
				BgItem.clearHashTable();
				GameCanvas.endDlg();
				CreateCharScr.isCreateChar = true;
				CreateCharScr.gI().switchToMe();
				break;
			case -107:
			{
				sbyte b26 = msg.reader().readByte();
				if (b26 == 0)
				{
					Char.myCharz().havePet = false;
				}
				if (b26 == 1)
				{
					Char.myCharz().havePet = true;
				}
				if (b26 != 2)
				{
					break;
				}
				InfoDlg.hide();
				Char.myPetz().head = msg.reader().readShort();
				Char.myPetz().setDefaultPart();
				int num53 = msg.reader().readUnsignedByte();
				Res.outz("num body = " + num53);
				Char.myPetz().arrItemBody = new Item[num53];
				for (int num54 = 0; num54 < num53; num54++)
				{
					short num55 = msg.reader().readShort();
					Res.outz("template id= " + num55);
					if (num55 == -1)
					{
						continue;
					}
					Res.outz("1");
					Char.myPetz().arrItemBody[num54] = new Item();
					Char.myPetz().arrItemBody[num54].template = ItemTemplates.get(num55);
					int num56 = Char.myPetz().arrItemBody[num54].template.type;
					Char.myPetz().arrItemBody[num54].quantity = msg.reader().readInt();
					Res.outz("3");
					Char.myPetz().arrItemBody[num54].info = msg.reader().readUTF();
					Char.myPetz().arrItemBody[num54].content = msg.reader().readUTF();
					int num57 = msg.reader().readUnsignedByte();
					Res.outz("option size= " + num57);
					if (num57 != 0)
					{
						Char.myPetz().arrItemBody[num54].itemOption = new ItemOption[num57];
						for (int num58 = 0; num58 < Char.myPetz().arrItemBody[num54].itemOption.Length; num58++)
						{
							int num59 = msg.reader().readUnsignedByte();
							int param4 = msg.reader().readUnsignedShort();
							if (num59 != -1)
							{
								Char.myPetz().arrItemBody[num54].itemOption[num58] = new ItemOption(num59, param4);
							}
						}
					}
					switch (num56)
					{
					case 0:
						Char.myPetz().body = Char.myPetz().arrItemBody[num54].template.part;
						break;
					case 1:
						Char.myPetz().leg = Char.myPetz().arrItemBody[num54].template.part;
						break;
					}
				}
				Char.myPetz().cHP = msg.readInt3Byte();
				Char.myPetz().cHPFull = msg.readInt3Byte();
				Char.myPetz().cMP = msg.readInt3Byte();
				Char.myPetz().cMPFull = msg.readInt3Byte();
				Char.myPetz().cDamFull = msg.readInt3Byte();
				Char.myPetz().cName = msg.reader().readUTF();
				Char.myPetz().currStrLevel = msg.reader().readUTF();
				Char.myPetz().cPower = msg.reader().readLong();
				Char.myPetz().cTiemNang = msg.reader().readLong();
				Char.myPetz().petStatus = msg.reader().readByte();
				Char.myPetz().cStamina = msg.reader().readShort();
				Char.myPetz().cMaxStamina = msg.reader().readShort();
				Char.myPetz().cCriticalFull = msg.reader().readByte();
				Char.myPetz().cDefull = msg.reader().readShort();
				Char.myPetz().arrPetSkill = new Skill[msg.reader().readByte()];
				Res.outz("SKILLENT = " + Char.myPetz().arrPetSkill);
				for (int num60 = 0; num60 < Char.myPetz().arrPetSkill.Length; num60++)
				{
					short num61 = msg.reader().readShort();
					if (num61 != -1)
					{
						Char.myPetz().arrPetSkill[num60] = Skills.get(num61);
						continue;
					}
					Char.myPetz().arrPetSkill[num60] = new Skill();
					Char.myPetz().arrPetSkill[num60].template = null;
					Char.myPetz().arrPetSkill[num60].moreInfo = msg.reader().readUTF();
				}
				if (GameCanvas.w > 2 * Panel.WIDTH_PANEL)
				{
					GameCanvas.panel2 = new Panel();
					GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
					GameCanvas.panel2.setTypeBodyOnly();
					GameCanvas.panel2.show();
					GameCanvas.panel.setTypePetMain();
					GameCanvas.panel.show();
				}
				else
				{
					GameCanvas.panel.tabName[21] = mResources.petMainTab;
					GameCanvas.panel.setTypePetMain();
					GameCanvas.panel.show();
				}
				break;
			}
			case -37:
			{
				sbyte b16 = msg.reader().readByte();
				Res.outz("cAction= " + b16);
				if (b16 != 0)
				{
					break;
				}
				Char.myCharz().head = msg.reader().readShort();
				Char.myCharz().setDefaultPart();
				int num31 = msg.reader().readUnsignedByte();
				Res.outz("num body = " + num31);
				Char.myCharz().arrItemBody = new Item[num31];
				for (int num32 = 0; num32 < num31; num32++)
				{
					short num33 = msg.reader().readShort();
					if (num33 == -1)
					{
						continue;
					}
					Char.myCharz().arrItemBody[num32] = new Item();
					Char.myCharz().arrItemBody[num32].template = ItemTemplates.get(num33);
					int num34 = Char.myCharz().arrItemBody[num32].template.type;
					Char.myCharz().arrItemBody[num32].quantity = msg.reader().readInt();
					Char.myCharz().arrItemBody[num32].info = msg.reader().readUTF();
					Char.myCharz().arrItemBody[num32].content = msg.reader().readUTF();
					int num35 = msg.reader().readUnsignedByte();
					if (num35 != 0)
					{
						Char.myCharz().arrItemBody[num32].itemOption = new ItemOption[num35];
						for (int num36 = 0; num36 < Char.myCharz().arrItemBody[num32].itemOption.Length; num36++)
						{
							int num37 = msg.reader().readUnsignedByte();
							int param2 = msg.reader().readUnsignedShort();
							if (num37 != -1)
							{
								Char.myCharz().arrItemBody[num32].itemOption[num36] = new ItemOption(num37, param2);
							}
						}
					}
					switch (num34)
					{
					case 0:
						Char.myCharz().body = Char.myCharz().arrItemBody[num32].template.part;
						break;
					case 1:
						Char.myCharz().leg = Char.myCharz().arrItemBody[num32].template.part;
						break;
					}
				}
				break;
			}
			case -36:
			{
				sbyte b8 = msg.reader().readByte();
				Res.outz("cAction= " + b8);
				if (b8 == 0)
				{
					int num16 = msg.reader().readUnsignedByte();
					Char.myCharz().arrItemBag = new Item[num16];
					GameScr.hpPotion = 0;
					Res.outz("numC=" + num16);
					for (int i = 0; i < num16; i++)
					{
						short num17 = msg.reader().readShort();
						if (num17 == -1)
						{
							continue;
						}
						Char.myCharz().arrItemBag[i] = new Item();
						Char.myCharz().arrItemBag[i].template = ItemTemplates.get(num17);
						Char.myCharz().arrItemBag[i].quantity = msg.reader().readInt();
						Char.myCharz().arrItemBag[i].info = msg.reader().readUTF();
						Char.myCharz().arrItemBag[i].content = msg.reader().readUTF();
						Char.myCharz().arrItemBag[i].indexUI = i;
						int num18 = msg.reader().readUnsignedByte();
						if (num18 != 0)
						{
							Char.myCharz().arrItemBag[i].itemOption = new ItemOption[num18];
							for (int j = 0; j < Char.myCharz().arrItemBag[i].itemOption.Length; j++)
							{
								int num19 = msg.reader().readUnsignedByte();
								int param = msg.reader().readUnsignedShort();
								if (num19 != -1)
								{
									Char.myCharz().arrItemBag[i].itemOption[j] = new ItemOption(num19, param);
								}
							}
							Char.myCharz().arrItemBag[i].compare = GameCanvas.panel.getCompare(Char.myCharz().arrItemBag[i]);
						}
						if (Char.myCharz().arrItemBag[i].template.type == 11)
						{
						}
						if (Char.myCharz().arrItemBag[i].template.type == 6)
						{
							GameScr.hpPotion += Char.myCharz().arrItemBag[i].quantity;
						}
					}
				}
				if (b8 == 2)
				{
					sbyte b9 = msg.reader().readByte();
					sbyte b10 = msg.reader().readByte();
					int quantity = Char.myCharz().arrItemBag[b9].quantity;
					Char.myCharz().arrItemBag[b9].quantity = b10;
					if (Char.myCharz().arrItemBag[b9].quantity < quantity && Char.myCharz().arrItemBag[b9].template.type == 6)
					{
						GameScr.hpPotion -= quantity - Char.myCharz().arrItemBag[b9].quantity;
					}
					if (Char.myCharz().arrItemBag[b9].quantity == 0)
					{
						Char.myCharz().arrItemBag[b9] = null;
					}
				}
				break;
			}
			case -35:
			{
				sbyte b54 = msg.reader().readByte();
				Res.outz("cAction= " + b54);
				if (b54 == 0)
				{
					int num142 = msg.reader().readUnsignedByte();
					Char.myCharz().arrItemBox = new Item[num142];
					GameCanvas.panel.hasUse = 0;
					for (int num143 = 0; num143 < num142; num143++)
					{
						short num144 = msg.reader().readShort();
						if (num144 == -1)
						{
							continue;
						}
						Char.myCharz().arrItemBox[num143] = new Item();
						Char.myCharz().arrItemBox[num143].template = ItemTemplates.get(num144);
						Char.myCharz().arrItemBox[num143].quantity = msg.reader().readInt();
						Char.myCharz().arrItemBox[num143].info = msg.reader().readUTF();
						Char.myCharz().arrItemBox[num143].content = msg.reader().readUTF();
						int num145 = msg.reader().readUnsignedByte();
						if (num145 != 0)
						{
							Char.myCharz().arrItemBox[num143].itemOption = new ItemOption[num145];
							for (int num146 = 0; num146 < Char.myCharz().arrItemBox[num143].itemOption.Length; num146++)
							{
								int num147 = msg.reader().readUnsignedByte();
								int param6 = msg.reader().readUnsignedShort();
								if (num147 != -1)
								{
									Char.myCharz().arrItemBox[num143].itemOption[num146] = new ItemOption(num147, param6);
								}
							}
						}
						GameCanvas.panel.hasUse++;
					}
				}
				if (b54 == 1)
				{
					bool isBoxClan = false;
					try
					{
						sbyte b55 = msg.reader().readByte();
						if (b55 == 1)
						{
							isBoxClan = true;
						}
					}
					catch (Exception)
					{
					}
					GameCanvas.panel.setTypeBox();
					GameCanvas.panel.isBoxClan = isBoxClan;
					GameCanvas.panel.show();
				}
				if (b54 == 2)
				{
					sbyte b56 = msg.reader().readByte();
					sbyte b57 = msg.reader().readByte();
					Char.myCharz().arrItemBox[b56].quantity = b57;
					if (Char.myCharz().arrItemBox[b56].quantity == 0)
					{
						Char.myCharz().arrItemBox[b56] = null;
					}
				}
				break;
			}
			case -45:
			{
				sbyte b62 = msg.reader().readByte();
				int num153 = msg.reader().readInt();
				short num154 = msg.reader().readShort();
				Res.outz("skill type= " + b62 + "   player use= " + num153);
				if (b62 == 0)
				{
					Res.outz("id use= " + num153);
					if (Char.myCharz().charID != num153)
					{
						@char = GameScr.findCharInMap(num153);
						if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
						{
							@char.setSkillPaint(GameScr.sks[num154], 0);
						}
						else
						{
							@char.setSkillPaint(GameScr.sks[num154], 1);
							@char.delayFall = 20;
						}
					}
					else
					{
						Char.myCharz().saveLoadPreviousSkill();
						Res.outz("LOAD LAST SKILL");
					}
					sbyte b63 = msg.reader().readByte();
					Res.outz("npc size= " + b63);
					for (int num155 = 0; num155 < b63; num155++)
					{
						sbyte b64 = msg.reader().readByte();
						sbyte b65 = msg.reader().readByte();
						Res.outz("index= " + b64);
						if (num154 >= 42 && num154 <= 48)
						{
							((Mob)GameScr.vMob.elementAt(b64)).isFreez = true;
							((Mob)GameScr.vMob.elementAt(b64)).seconds = b65;
							((Mob)GameScr.vMob.elementAt(b64)).last = (((Mob)GameScr.vMob.elementAt(b64)).cur = mSystem.currentTimeMillis());
						}
					}
					sbyte b66 = msg.reader().readByte();
					for (int num156 = 0; num156 < b66; num156++)
					{
						int num157 = msg.reader().readInt();
						sbyte b67 = msg.reader().readByte();
						Res.outz("player ID= " + num157 + " my ID= " + Char.myCharz().charID);
						if (num154 < 42 || num154 > 48)
						{
							continue;
						}
						if (num157 == Char.myCharz().charID)
						{
							if (!Char.myCharz().isFlyAndCharge && !Char.myCharz().isStandAndCharge)
							{
								GameScr.gI().isFreez = true;
								Char.myCharz().isFreez = true;
								Char.myCharz().freezSeconds = b67;
								Char.myCharz().lastFreez = (Char.myCharz().currFreez = mSystem.currentTimeMillis());
								Char.myCharz().isLockMove = true;
							}
						}
						else
						{
							@char = GameScr.findCharInMap(num157);
							if (@char != null && !@char.isFlyAndCharge && !@char.isStandAndCharge)
							{
								@char.isFreez = true;
								@char.seconds = b67;
								@char.freezSeconds = b67;
								@char.lastFreez = (GameScr.findCharInMap(num157).currFreez = mSystem.currentTimeMillis());
							}
						}
					}
				}
				if (b62 == 1 && num153 != Char.myCharz().charID)
				{
					GameScr.findCharInMap(num153).isCharge = true;
				}
				if (b62 == 3)
				{
					if (num153 == Char.myCharz().charID)
					{
						Char.myCharz().isCharge = false;
						SoundMn.gI().taitaoPause();
						Char.myCharz().saveLoadPreviousSkill();
					}
					else
					{
						GameScr.findCharInMap(num153).isCharge = false;
					}
				}
				if (b62 == 4)
				{
					if (num153 == Char.myCharz().charID)
					{
						Char.myCharz().seconds = msg.reader().readShort() - 1000;
						Char.myCharz().last = mSystem.currentTimeMillis();
						Res.outz("second= " + Char.myCharz().seconds + " last= " + Char.myCharz().last);
					}
					else if (GameScr.findCharInMap(num153) != null)
					{
						switch (GameScr.findCharInMap(num153).cgender)
						{
						case 0:
							GameScr.findCharInMap(num153).useChargeSkill(isGround: false);
							break;
						case 1:
							GameScr.findCharInMap(num153).useChargeSkill(isGround: true);
							break;
						}
						GameScr.findCharInMap(num153).skillTemplateId = num154;
						GameScr.findCharInMap(num153).isUseSkillAfterCharge = true;
						GameScr.findCharInMap(num153).seconds = msg.reader().readShort();
						GameScr.findCharInMap(num153).last = mSystem.currentTimeMillis();
					}
				}
				if (b62 == 5)
				{
					if (num153 == Char.myCharz().charID)
					{
						Char.myCharz().stopUseChargeSkill();
					}
					else if (GameScr.findCharInMap(num153) != null)
					{
						GameScr.findCharInMap(num153).stopUseChargeSkill();
					}
				}
				if (b62 == 6)
				{
					if (num153 == Char.myCharz().charID)
					{
						Char.myCharz().setAutoSkillPaint(GameScr.sks[num154], 0);
					}
					else if (GameScr.findCharInMap(num153) != null)
					{
						GameScr.findCharInMap(num153).setAutoSkillPaint(GameScr.sks[num154], 0);
						SoundMn.gI().gong();
					}
				}
				if (b62 == 7)
				{
					if (num153 == Char.myCharz().charID)
					{
						Char.myCharz().seconds = msg.reader().readShort();
						Res.outz("second = " + Char.myCharz().seconds);
						Char.myCharz().last = mSystem.currentTimeMillis();
					}
					else if (GameScr.findCharInMap(num153) != null)
					{
						GameScr.findCharInMap(num153).useChargeSkill(isGround: true);
						GameScr.findCharInMap(num153).seconds = msg.reader().readShort();
						GameScr.findCharInMap(num153).last = mSystem.currentTimeMillis();
						SoundMn.gI().gong();
					}
				}
				if (b62 == 8 && num153 != Char.myCharz().charID && GameScr.findCharInMap(num153) != null)
				{
					GameScr.findCharInMap(num153).setAutoSkillPaint(GameScr.sks[num154], 0);
				}
				break;
			}
			case -44:
			{
				bool flag8 = false;
				if (GameCanvas.w > 2 * Panel.WIDTH_PANEL)
				{
					flag8 = true;
				}
				sbyte b42 = msg.reader().readByte();
				int num93 = msg.reader().readUnsignedByte();
				Char.myCharz().arrItemShop = new Item[num93][];
				GameCanvas.panel.shopTabName = new string[num93 + ((!flag8) ? 1 : 0)][];
				for (int num94 = 0; num94 < GameCanvas.panel.shopTabName.Length; num94++)
				{
					GameCanvas.panel.shopTabName[num94] = new string[2];
				}
				if (b42 == 2)
				{
					GameCanvas.panel.maxPageShop = new int[num93];
					GameCanvas.panel.currPageShop = new int[num93];
				}
				if (!flag8)
				{
					GameCanvas.panel.shopTabName[num93] = mResources.inventory;
				}
				for (int num95 = 0; num95 < num93; num95++)
				{
					string[] array8 = Res.split(msg.reader().readUTF(), "\n", 0);
					if (b42 == 2)
					{
						GameCanvas.panel.maxPageShop[num95] = msg.reader().readUnsignedByte();
					}
					if (array8.Length == 2)
					{
						GameCanvas.panel.shopTabName[num95] = array8;
					}
					if (array8.Length == 1)
					{
						GameCanvas.panel.shopTabName[num95][0] = array8[0];
						GameCanvas.panel.shopTabName[num95][1] = string.Empty;
					}
					int num96 = msg.reader().readUnsignedByte();
					Char.myCharz().arrItemShop[num95] = new Item[num96];
					Panel.strWantToBuy = mResources.say_wat_do_u_want_to_buy;
					if (b42 == 1)
					{
						Panel.strWantToBuy = mResources.say_wat_do_u_want_to_buy2;
					}
					for (int num97 = 0; num97 < num96; num97++)
					{
						short num98 = msg.reader().readShort();
						if (num98 == -1)
						{
							continue;
						}
						Char.myCharz().arrItemShop[num95][num97] = new Item();
						Char.myCharz().arrItemShop[num95][num97].template = ItemTemplates.get(num98);
						Res.outz("name " + num95 + " = " + Char.myCharz().arrItemShop[num95][num97].template.name + " id templat= " + Char.myCharz().arrItemShop[num95][num97].template.id);
						if (b42 == 4)
						{
							Char.myCharz().arrItemShop[num95][num97].reason = msg.reader().readUTF();
						}
						else if (b42 == 0)
						{
							Char.myCharz().arrItemShop[num95][num97].buyCoin = msg.reader().readInt();
							Char.myCharz().arrItemShop[num95][num97].buyGold = msg.reader().readInt();
						}
						else if (b42 == 1)
						{
							Char.myCharz().arrItemShop[num95][num97].powerRequire = msg.reader().readLong();
						}
						else if (b42 == 2)
						{
							Char.myCharz().arrItemShop[num95][num97].itemId = msg.reader().readShort();
							Char.myCharz().arrItemShop[num95][num97].buyCoin = msg.reader().readInt();
							Char.myCharz().arrItemShop[num95][num97].buyGold = msg.reader().readInt();
							Char.myCharz().arrItemShop[num95][num97].buyType = msg.reader().readByte();
							Char.myCharz().arrItemShop[num95][num97].quantity = msg.reader().readByte();
							Char.myCharz().arrItemShop[num95][num97].isMe = msg.reader().readByte();
						}
						else if (b42 == 3)
						{
							Char.myCharz().arrItemShop[num95][num97].isBuySpec = true;
							Char.myCharz().arrItemShop[num95][num97].iconSpec = msg.reader().readShort();
							Char.myCharz().arrItemShop[num95][num97].buySpec = msg.reader().readInt();
						}
						int num99 = msg.reader().readUnsignedByte();
						if (num99 != 0)
						{
							Char.myCharz().arrItemShop[num95][num97].itemOption = new ItemOption[num99];
							for (int num100 = 0; num100 < Char.myCharz().arrItemShop[num95][num97].itemOption.Length; num100++)
							{
								int num101 = msg.reader().readUnsignedByte();
								int param5 = msg.reader().readUnsignedShort();
								if (num101 != -1)
								{
									Char.myCharz().arrItemShop[num95][num97].itemOption[num100] = new ItemOption(num101, param5);
									Char.myCharz().arrItemShop[num95][num97].compare = GameCanvas.panel.getCompare(Char.myCharz().arrItemShop[num95][num97]);
								}
							}
						}
						sbyte b43 = msg.reader().readByte();
						Char.myCharz().arrItemShop[num95][num97].newItem = ((b43 != 0) ? true : false);
						sbyte b44 = msg.reader().readByte();
						if (b44 == 1)
						{
							int headTemp = msg.reader().readShort();
							int bodyTemp = msg.reader().readShort();
							int legTemp = msg.reader().readShort();
							int bagTemp = msg.reader().readShort();
							Char.myCharz().arrItemShop[num95][num97].setPartTemp(headTemp, bodyTemp, legTemp, bagTemp);
						}
					}
				}
				if (flag8)
				{
					if (b42 != 2)
					{
						GameCanvas.panel2 = new Panel();
						GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
						GameCanvas.panel2.setTypeBodyOnly();
						GameCanvas.panel2.show();
					}
					else
					{
						GameCanvas.panel2 = new Panel();
						GameCanvas.panel2.setTypeKiGuiOnly();
						GameCanvas.panel2.show();
					}
				}
				GameCanvas.panel.tabName[1] = GameCanvas.panel.shopTabName;
				if (b42 == 2)
				{
					string[][] array9 = GameCanvas.panel.tabName[1];
					if (flag8)
					{
						GameCanvas.panel.tabName[1] = new string[4][]
						{
							array9[0],
							array9[1],
							array9[2],
							array9[3]
						};
					}
					else
					{
						GameCanvas.panel.tabName[1] = new string[5][]
						{
							array9[0],
							array9[1],
							array9[2],
							array9[3],
							array9[4]
						};
					}
				}
				GameCanvas.panel.setTypeShop(b42);
				GameCanvas.panel.show();
				break;
			}
			case -41:
			{
				sbyte b41 = msg.reader().readByte();
				Char.myCharz().strLevel = new string[b41];
				for (int num89 = 0; num89 < b41; num89++)
				{
					string text2 = msg.reader().readUTF();
					Char.myCharz().strLevel[num89] = text2;
				}
				Res.outz("---   xong  level caption cmd : " + msg.command);
				break;
			}
			case -34:
			{
				sbyte b23 = msg.reader().readByte();
				Res.outz("act= " + b23);
				if (b23 == 0 && GameScr.gI().magicTree != null)
				{
					Res.outz("toi duoc day");
					MagicTree magicTree = GameScr.gI().magicTree;
					magicTree.id = msg.reader().readShort();
					magicTree.name = msg.reader().readUTF();
					magicTree.name = Res.changeString(magicTree.name);
					magicTree.x = msg.reader().readShort();
					magicTree.y = msg.reader().readShort();
					magicTree.level = msg.reader().readByte();
					magicTree.currPeas = msg.reader().readShort();
					magicTree.maxPeas = msg.reader().readShort();
					Res.outz("curr Peas= " + magicTree.currPeas);
					magicTree.strInfo = msg.reader().readUTF();
					magicTree.seconds = msg.reader().readInt();
					magicTree.timeToRecieve = magicTree.seconds;
					sbyte b24 = msg.reader().readByte();
					magicTree.peaPostionX = new int[b24];
					magicTree.peaPostionY = new int[b24];
					for (int num51 = 0; num51 < b24; num51++)
					{
						magicTree.peaPostionX[num51] = msg.reader().readByte();
						magicTree.peaPostionY[num51] = msg.reader().readByte();
					}
					magicTree.isUpdate = msg.reader().readBool();
					magicTree.last = (magicTree.cur = mSystem.currentTimeMillis());
					GameScr.gI().magicTree.isUpdateTree = true;
				}
				if (b23 == 1)
				{
					myVector = new MyVector();
					try
					{
						while (msg.reader().available() > 0)
						{
							string caption = msg.reader().readUTF();
							myVector.addElement(new Command(caption, GameCanvas.instance, 888392, null));
						}
					}
					catch (Exception ex7)
					{
						Cout.println("Loi MAGIC_TREE " + ex7.ToString());
					}
					GameCanvas.menu.startAt(myVector, 3);
				}
				if (b23 == 2)
				{
					GameScr.gI().magicTree.remainPeas = msg.reader().readShort();
					GameScr.gI().magicTree.seconds = msg.reader().readInt();
					GameScr.gI().magicTree.last = (GameScr.gI().magicTree.cur = mSystem.currentTimeMillis());
					GameScr.gI().magicTree.isUpdateTree = true;
					GameScr.gI().magicTree.isPeasEffect = true;
				}
				break;
			}
			case 11:
			{
				GameCanvas.debug("SA9", 2);
				int num28 = msg.reader().readByte();
				sbyte b14 = msg.reader().readByte();
				if (b14 != 0)
				{
					Mob.arrMobTemplate[num28].data.readDataNewBoss(NinjaUtil.readByteArray(msg), b14);
				}
				else
				{
					Mob.arrMobTemplate[num28].data.readData(NinjaUtil.readByteArray(msg));
				}
				for (int m = 0; m < GameScr.vMob.size(); m++)
				{
					mob = (Mob)GameScr.vMob.elementAt(m);
					if (mob.templateId == num28)
					{
						mob.w = Mob.arrMobTemplate[num28].data.width;
						mob.h = Mob.arrMobTemplate[num28].data.height;
					}
				}
				sbyte[] array5 = NinjaUtil.readByteArray(msg);
				Image img = Image.createImage(array5, 0, array5.Length);
				Mob.arrMobTemplate[num28].data.img = img;
				int num29 = msg.reader().readByte();
				if (num29 == 1)
				{
					readFrameBoss(msg, num28);
				}
				break;
			}
			case -69:
				Char.myCharz().cMaxStamina = msg.reader().readShort();
				break;
			case -68:
				Char.myCharz().cStamina = msg.reader().readShort();
				break;
			case -67:
			{
				Res.outz("RECIEVE ICON");
				demCount += 1f;
				int num26 = msg.reader().readInt();
				sbyte[] array4 = null;
				try
				{
					array4 = NinjaUtil.readByteArray(msg);
					Res.outz("request hinh icon = " + num26);
					if (num26 == 3896)
					{
						Res.outz("SIZE CHECK= " + array4.Length);
					}
					SmallImage.imgNew[num26].img = createImage(array4);
				}
				catch (Exception)
				{
					array4 = null;
					SmallImage.imgNew[num26].img = Image.createRGBImage(new int[1], 1, 1, bl: true);
				}
				if (array4 != null && mGraphics.zoomLevel > 1)
				{
					Rms.saveRMS(mGraphics.zoomLevel + "Small" + num26, array4);
				}
				break;
			}
			case -66:
			{
				short id = msg.reader().readShort();
				sbyte[] data = NinjaUtil.readByteArray(msg);
				EffectData effDataById = Effect.getEffDataById(id);
				effDataById.readData(data);
				sbyte[] array2 = NinjaUtil.readByteArray(msg);
				effDataById.img = Image.createImage(array2, 0, array2.Length);
				break;
			}
			case -32:
			{
				if (GameCanvas.lowGraphic && TileMap.mapID != 51 && TileMap.mapID != 103)
				{
					return;
				}
				short num161 = msg.reader().readShort();
				int num162 = msg.reader().readInt();
				sbyte[] array16 = null;
				Image image = null;
				try
				{
					array16 = new sbyte[num162];
					for (int num163 = 0; num163 < num162; num163++)
					{
						array16[num163] = msg.reader().readByte();
					}
					image = Image.createImage(array16, 0, num162);
					BgItem.imgNew.put(num161 + string.Empty, image);
				}
				catch (Exception)
				{
					array16 = null;
					BgItem.imgNew.put(num161 + string.Empty, Image.createRGBImage(new int[1], 1, 1, bl: true));
				}
				if (array16 != null)
				{
					if (mGraphics.zoomLevel > 1)
					{
						Rms.saveRMS(mGraphics.zoomLevel + "bgItem" + num161, array16);
					}
					BgItemMn.blendcurrBg(num161, image);
				}
				break;
			}
			case 92:
			{
				if (GameCanvas.currentScreen == GameScr.instance)
				{
					GameCanvas.endDlg();
				}
				string text4 = msg.reader().readUTF();
				string str2 = msg.reader().readUTF();
				str2 = Res.changeString(str2);
				string empty = string.Empty;
				Char char11 = null;
				sbyte b52 = 0;
				if (!text4.Equals(string.Empty))
				{
					char11 = new Char();
					char11.charID = msg.reader().readInt();
					char11.head = msg.reader().readShort();
					char11.body = msg.reader().readShort();
					char11.bag = msg.reader().readShort();
					char11.leg = msg.reader().readShort();
					b52 = msg.reader().readByte();
					char11.cName = text4;
				}
				empty += str2;
				InfoDlg.hide();
				if (text4.Equals(string.Empty))
				{
					GameScr.info1.addInfo(empty, 0);
					break;
				}
				GameScr.info2.addInfoWithChar(empty, char11, (b52 == 0) ? true : false);
				if (GameCanvas.panel.isShow && GameCanvas.panel.type == 8)
				{
					GameCanvas.panel.initLogMessage();
				}
				break;
			}
			case -26:
				ServerListScreen.testConnect = 2;
				GameCanvas.debug("SA2", 2);
				GameCanvas.startOKDlg(msg.reader().readUTF());
				InfoDlg.hide();
				LoginScr.isContinueToLogin = false;
				Char.isLoadingMap = false;
				if (GameCanvas.currentScreen == GameCanvas.loginScr)
				{
					GameCanvas.serverScreen.switchToMe();
				}
				break;
			case -25:
				GameCanvas.debug("SA3", 2);
				GameScr.info1.addInfo(msg.reader().readUTF(), 0);
				break;
			case 94:
				GameCanvas.debug("SA3", 2);
				GameScr.info1.addInfo(msg.reader().readUTF(), 0);
				break;
			case 47:
				GameCanvas.debug("SA4", 2);
				GameScr.gI().resetButton();
				break;
			case 81:
			{
				GameCanvas.debug("SXX4", 2);
				Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				mob7.isDisable = msg.reader().readBool();
				break;
			}
			case 82:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				mob7.isDontMove = msg.reader().readBool();
				break;
			}
			case 85:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				mob7.isFire = msg.reader().readBool();
				break;
			}
			case 86:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				mob7.isIce = msg.reader().readBool();
				if (!mob7.isIce)
				{
					ServerEffect.addServerEffect(77, mob7.x, mob7.y - 9, 1);
				}
				break;
			}
			case 87:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				mob7.isWind = msg.reader().readBool();
				break;
			}
			case 56:
			{
				GameCanvas.debug("SXX6", 2);
				@char = null;
				int num13 = msg.reader().readInt();
				if (num13 == Char.myCharz().charID)
				{
					bool flag4 = false;
					@char = Char.myCharz();
					@char.cHP = msg.readInt3Byte();
					int num62 = msg.readInt3Byte();
					Res.outz("dame hit = " + num62);
					if (num62 != 0)
					{
						@char.doInjure();
					}
					int num63 = 0;
					try
					{
						flag4 = msg.reader().readBoolean();
						sbyte b27 = msg.reader().readByte();
						if (b27 != -1)
						{
							Res.outz("hit eff= " + b27);
							EffecMn.addEff(new Effect(b27, @char.cx, @char.cy, 3, 1, -1));
						}
					}
					catch (Exception)
					{
					}
					num62 += num63;
					if (Char.myCharz().cTypePk != 4)
					{
						if (num62 == 0)
						{
							GameScr.startFlyText(mResources.miss, @char.cx, @char.cy - @char.ch, 0, -3, mFont.MISS_ME);
						}
						else
						{
							GameScr.startFlyText("-" + num62, @char.cx, @char.cy - @char.ch, 0, -3, flag4 ? mFont.FATAL : mFont.RED);
						}
					}
					break;
				}
				@char = GameScr.findCharInMap(num13);
				if (@char == null)
				{
					return;
				}
				@char.cHP = msg.readInt3Byte();
				bool flag5 = false;
				int num64 = msg.readInt3Byte();
				if (num64 != 0)
				{
					@char.doInjure();
				}
				int num65 = 0;
				try
				{
					flag5 = msg.reader().readBoolean();
					sbyte b28 = msg.reader().readByte();
					if (b28 != -1)
					{
						Res.outz("hit eff= " + b28);
						EffecMn.addEff(new Effect(b28, @char.cx, @char.cy, 3, 1, -1));
					}
				}
				catch (Exception)
				{
				}
				num64 += num65;
				if (@char.cTypePk != 4)
				{
					if (num64 == 0)
					{
						GameScr.startFlyText(mResources.miss, @char.cx, @char.cy - @char.ch, 0, -3, mFont.MISS);
					}
					else
					{
						GameScr.startFlyText("-" + num64, @char.cx, @char.cy - @char.ch, 0, -3, flag5 ? mFont.FATAL : mFont.ORANGE);
					}
				}
				break;
			}
			case 83:
			{
				GameCanvas.debug("SXX8", 2);
				int num13 = msg.reader().readInt();
				@char = ((num13 != Char.myCharz().charID) ? GameScr.findCharInMap(num13) : Char.myCharz());
				if (@char == null)
				{
					return;
				}
				Mob mobToAttack = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				if (@char.mobMe != null)
				{
					@char.mobMe.attackOtherMob(mobToAttack);
				}
				break;
			}
			case 84:
			{
				int num13 = msg.reader().readInt();
				if (num13 == Char.myCharz().charID)
				{
					@char = Char.myCharz();
				}
				else
				{
					@char = GameScr.findCharInMap(num13);
					if (@char == null)
					{
						return;
					}
				}
				@char.cHP = @char.cHPFull;
				@char.cMP = @char.cMPFull;
				@char.cx = msg.reader().readShort();
				@char.cy = msg.reader().readShort();
				@char.liveFromDead();
				break;
			}
			case 46:
				GameCanvas.debug("SA5", 2);
				Cout.LogWarning("Controler RESET_POINT  " + Char.ischangingMap);
				Char.isLockKey = false;
				Char.myCharz().setResetPoint(msg.reader().readShort(), msg.reader().readShort());
				break;
			case -29:
				messageNotLogin(msg);
				break;
			case -28:
				messageNotMap(msg);
				break;
			case -30:
				messageSubCommand(msg);
				break;
			case 62:
				GameCanvas.debug("SZ3", 2);
				@char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char != null)
				{
					@char.killCharId = Char.myCharz().charID;
					Char.myCharz().npcFocus = null;
					Char.myCharz().mobFocus = null;
					Char.myCharz().itemFocus = null;
					Char.myCharz().charFocus = @char;
					Char.isManualFocus = true;
					GameScr.info1.addInfo(@char.cName + mResources.CUU_SAT, 0);
				}
				break;
			case 63:
				GameCanvas.debug("SZ4", 2);
				Char.myCharz().killCharId = msg.reader().readInt();
				Char.myCharz().npcFocus = null;
				Char.myCharz().mobFocus = null;
				Char.myCharz().itemFocus = null;
				Char.myCharz().charFocus = GameScr.findCharInMap(Char.myCharz().killCharId);
				Char.isManualFocus = true;
				break;
			case 64:
				GameCanvas.debug("SZ5", 2);
				@char = Char.myCharz();
				try
				{
					@char = GameScr.findCharInMap(msg.reader().readInt());
				}
				catch (Exception ex4)
				{
					Cout.println("Loi CLEAR_CUU_SAT " + ex4.ToString());
				}
				@char.killCharId = -9999;
				break;
			case 39:
				GameCanvas.debug("SA49", 2);
				GameScr.gI().typeTradeOrder = 2;
				if (GameScr.gI().typeTrade >= 2 && GameScr.gI().typeTradeOrder >= 2)
				{
					InfoDlg.showWait();
				}
				break;
			case 57:
			{
				GameCanvas.debug("SZ6", 2);
				MyVector myVector2 = new MyVector();
				myVector2.addElement(new Command(msg.reader().readUTF(), GameCanvas.instance, 88817, null));
				GameCanvas.menu.startAt(myVector2, 3);
				break;
			}
			case 20:
			{
				GameCanvas.debug("SZ7", 2);
				mob = (Mob)GameScr.vMob.elementAt(msg.reader().readByte());
				int num13 = msg.reader().readInt();
				@char = ((num13 != Char.myCharz().charID) ? GameScr.findCharInMap(num13) : Char.myCharz());
				@char.moveFast = new short[3];
				@char.moveFast[0] = 0;
				@char.moveFast[1] = (short)mob.x;
				@char.moveFast[2] = (short)mob.y;
				break;
			}
			case 58:
			{
				GameCanvas.debug("SZ7", 2);
				int num13 = msg.reader().readInt();
				Char char4 = ((num13 != Char.myCharz().charID) ? GameScr.findCharInMap(num13) : Char.myCharz());
				char4.moveFast = new short[3];
				char4.moveFast[0] = 0;
				short num14 = msg.reader().readShort();
				short num15 = msg.reader().readShort();
				char4.moveFast[1] = num14;
				char4.moveFast[2] = num15;
				try
				{
					num13 = msg.reader().readInt();
					Char char5 = ((num13 != Char.myCharz().charID) ? GameScr.findCharInMap(num13) : Char.myCharz());
					char5.cx = num14;
					char5.cy = num15;
				}
				catch (Exception ex)
				{
					Cout.println("Loi MOVE_FAST " + ex.ToString());
				}
				break;
			}
			case 88:
			{
				string info = msg.reader().readUTF();
				short num27 = msg.reader().readShort();
				GameCanvas.inputDlg.show(info, new Command(mResources.ACCEPT, GameCanvas.instance, 88818, num27), TField.INPUT_TYPE_ANY);
				break;
			}
			case 27:
			{
				myVector = new MyVector();
				string text7 = msg.reader().readUTF();
				int num166 = msg.reader().readByte();
				for (int num167 = 0; num167 < num166; num167++)
				{
					string caption4 = msg.reader().readUTF();
					short num168 = msg.reader().readShort();
					myVector.addElement(new Command(caption4, GameCanvas.instance, 88819, num168));
				}
				GameCanvas.menu.startWithoutCloseButton(myVector, 3);
				break;
			}
			case 33:
			{
				GameCanvas.debug("SA51", 2);
				InfoDlg.hide();
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				myVector = new MyVector();
				try
				{
					while (true)
					{
						string caption3 = msg.reader().readUTF();
						myVector.addElement(new Command(caption3, GameCanvas.instance, 88822, null));
					}
				}
				catch (Exception ex18)
				{
					Cout.println("Loi OPEN_UI_MENU " + ex18.ToString());
				}
				if (Char.myCharz().npcFocus == null)
				{
					return;
				}
				for (int num160 = 0; num160 < Char.myCharz().npcFocus.template.menu.Length; num160++)
				{
					string[] array15 = Char.myCharz().npcFocus.template.menu[num160];
					myVector.addElement(new Command(array15[0], GameCanvas.instance, 88820, array15));
				}
				GameCanvas.menu.startAt(myVector, 3);
				break;
			}
			case 40:
			{
				GameCanvas.debug("SA52", 2);
				GameCanvas.taskTick = 150;
				short taskId = msg.reader().readShort();
				sbyte index3 = msg.reader().readByte();
				string str3 = msg.reader().readUTF();
				str3 = Res.changeString(str3);
				string str4 = msg.reader().readUTF();
				str4 = Res.changeString(str4);
				string[] array12 = new string[msg.reader().readByte()];
				string[] array13 = new string[array12.Length];
				GameScr.tasks = new int[array12.Length];
				GameScr.mapTasks = new int[array12.Length];
				short[] array14 = new short[array12.Length];
				short count = -1;
				for (int num151 = 0; num151 < array12.Length; num151++)
				{
					string str5 = msg.reader().readUTF();
					str5 = Res.changeString(str5);
					GameScr.tasks[num151] = msg.reader().readByte();
					GameScr.mapTasks[num151] = msg.reader().readShort();
					string str6 = msg.reader().readUTF();
					str6 = Res.changeString(str6);
					array14[num151] = -1;
					if (!str5.Equals(string.Empty))
					{
						array12[num151] = str5;
						array13[num151] = str6;
					}
				}
				try
				{
					count = msg.reader().readShort();
					for (int num152 = 0; num152 < array12.Length; num152++)
					{
						array14[num152] = msg.reader().readShort();
					}
				}
				catch (Exception ex17)
				{
					Cout.println("Loi TASK_GET " + ex17.ToString());
				}
				Char.myCharz().taskMaint = new Task(taskId, index3, str3, str4, array12, array14, count, array13);
				if (Char.myCharz().npcFocus != null)
				{
					Npc.clearEffTask();
				}
				Char.taskAction(isNextStep: false);
				break;
			}
			case 41:
				GameCanvas.debug("SA53", 2);
				GameCanvas.taskTick = 100;
				Res.outz("TASK NEXT");
				Char.myCharz().taskMaint.index++;
				Char.myCharz().taskMaint.count = 0;
				Npc.clearEffTask();
				Char.taskAction(isNextStep: true);
				break;
			case 50:
			{
				sbyte b60 = msg.reader().readByte();
				Panel.vGameInfo.removeAllElements();
				for (int num150 = 0; num150 < b60; num150++)
				{
					GameInfo gameInfo = new GameInfo();
					gameInfo.id = msg.reader().readShort();
					gameInfo.main = msg.reader().readUTF();
					gameInfo.content = msg.reader().readUTF();
					Panel.vGameInfo.addElement(gameInfo);
					bool flag10 = (gameInfo.hasRead = Rms.loadRMSInt(gameInfo.id + string.Empty) != -1);
				}
				break;
			}
			case 43:
				GameCanvas.taskTick = 50;
				GameCanvas.debug("SA55", 2);
				Char.myCharz().taskMaint.count = msg.reader().readShort();
				if (Char.myCharz().npcFocus != null)
				{
					Npc.clearEffTask();
				}
				break;
			case 90:
				GameCanvas.debug("SA577", 2);
				requestItemPlayer(msg);
				break;
			case 29:
				GameCanvas.debug("SA58", 2);
				GameScr.gI().openUIZone(msg);
				break;
			case -21:
			{
				GameCanvas.debug("SA60", 2);
				short itemMapID = msg.reader().readShort();
				for (int num141 = 0; num141 < GameScr.vItemMap.size(); num141++)
				{
					if (((ItemMap)GameScr.vItemMap.elementAt(num141)).itemMapID == itemMapID)
					{
						GameScr.vItemMap.removeElementAt(num141);
						break;
					}
				}
				break;
			}
			case -20:
			{
				GameCanvas.debug("SA61", 2);
				Char.myCharz().itemFocus = null;
				short itemMapID = msg.reader().readShort();
				for (int num139 = 0; num139 < GameScr.vItemMap.size(); num139++)
				{
					ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(num139);
					if (itemMap2.itemMapID != itemMapID)
					{
						continue;
					}
					itemMap2.setPoint(Char.myCharz().cx, Char.myCharz().cy - 10);
					string text5 = msg.reader().readUTF();
					num = 0;
					try
					{
						num = msg.reader().readShort();
						if (itemMap2.template.type == 9)
						{
							num = msg.reader().readShort();
							Char.myCharz().xu += num;
						}
						else if (itemMap2.template.type == 10)
						{
							num = msg.reader().readShort();
							Char.myCharz().luong += num;
						}
						else if (itemMap2.template.type == 34)
						{
							num = msg.reader().readShort();
							Char.myCharz().luongKhoa += num;
						}
					}
					catch (Exception)
					{
					}
					if (text5.Equals(string.Empty))
					{
						if (itemMap2.template.type == 9)
						{
							GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.YELLOW);
							SoundMn.gI().getItem();
						}
						else if (itemMap2.template.type == 10)
						{
							GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.GREEN);
							SoundMn.gI().getItem();
						}
						else if (itemMap2.template.type == 34)
						{
							GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.RED);
							SoundMn.gI().getItem();
						}
						else
						{
							GameScr.info1.addInfo(mResources.you_receive + " " + ((num <= 0) ? string.Empty : (num + " ")) + itemMap2.template.name, 0);
							SoundMn.gI().getItem();
						}
						if (num > 0 && Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 4683)
						{
							ServerEffect.addServerEffect(55, Char.myCharz().petFollow.cmx, Char.myCharz().petFollow.cmy, 1);
							ServerEffect.addServerEffect(55, Char.myCharz().cx, Char.myCharz().cy, 1);
						}
					}
					else if (text5.Length == 1)
					{
						Cout.LogError3("strInf.Length =1:  " + text5);
					}
					else
					{
						GameScr.info1.addInfo(text5, 0);
					}
					break;
				}
				break;
			}
			case -19:
			{
				GameCanvas.debug("SA62", 2);
				short itemMapID = msg.reader().readShort();
				@char = GameScr.findCharInMap(msg.reader().readInt());
				for (int num138 = 0; num138 < GameScr.vItemMap.size(); num138++)
				{
					ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(num138);
					if (itemMap.itemMapID != itemMapID)
					{
						continue;
					}
					if (@char == null)
					{
						return;
					}
					itemMap.setPoint(@char.cx, @char.cy - 10);
					if (itemMap.x < @char.cx)
					{
						@char.cdir = -1;
					}
					else if (itemMap.x > @char.cx)
					{
						@char.cdir = 1;
					}
					break;
				}
				break;
			}
			case -18:
			{
				GameCanvas.debug("SA63", 2);
				int num137 = msg.reader().readByte();
				GameScr.vItemMap.addElement(new ItemMap(msg.reader().readShort(), Char.myCharz().arrItemBag[num137].template.id, Char.myCharz().cx, Char.myCharz().cy, msg.reader().readShort(), msg.reader().readShort()));
				Char.myCharz().arrItemBag[num137] = null;
				break;
			}
			case 68:
			{
				Res.outz("ADD ITEM TO MAP --------------------------------------");
				GameCanvas.debug("SA6333", 2);
				short itemMapID = msg.reader().readShort();
				short itemTemplateID = msg.reader().readShort();
				int x = msg.reader().readShort();
				int y = msg.reader().readShort();
				int num134 = msg.reader().readInt();
				short r = 0;
				if (num134 == -2)
				{
					r = msg.reader().readShort();
				}
				ItemMap o2 = new ItemMap(num134, itemMapID, itemTemplateID, x, y, r);
				GameScr.vItemMap.addElement(o2);
				break;
			}
			case 69:
				GameCanvas.debug("SA633355", 2);
				Char.myCharz().arrItemBag[msg.reader().readByte()].quantity = msg.reader().readShort();
				break;
			case -14:
				GameCanvas.debug("SA64", 2);
				@char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char == null)
				{
					return;
				}
				GameScr.vItemMap.addElement(new ItemMap(msg.reader().readShort(), msg.reader().readShort(), @char.cx, @char.cy, msg.reader().readShort(), msg.reader().readShort()));
				break;
			case -22:
				GameCanvas.debug("SA65", 2);
				Char.isLockKey = true;
				Char.ischangingMap = true;
				GameScr.gI().timeStartMap = 0;
				GameScr.gI().timeLengthMap = 0;
				Char.myCharz().mobFocus = null;
				Char.myCharz().npcFocus = null;
				Char.myCharz().charFocus = null;
				Char.myCharz().itemFocus = null;
				Char.myCharz().focus.removeAllElements();
				Char.myCharz().testCharId = -9999;
				Char.myCharz().killCharId = -9999;
				GameCanvas.resetBg();
				GameScr.gI().resetButton();
				GameScr.gI().center = null;
				break;
			case -70:
			{
				Res.outz("BIG MESSAGE .......................................");
				GameCanvas.endDlg();
				int avatar = msg.reader().readShort();
				string chat3 = msg.reader().readUTF();
				Npc npc6 = new Npc(-1, 0, 0, 0, 0, 0);
				npc6.avatar = avatar;
				ChatPopup.addBigMessage(chat3, 100000, npc6);
				sbyte b50 = msg.reader().readByte();
				if (b50 == 0)
				{
					ChatPopup.serverChatPopUp.cmdMsg1 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null);
					ChatPopup.serverChatPopUp.cmdMsg1.x = GameCanvas.w / 2 - 35;
					ChatPopup.serverChatPopUp.cmdMsg1.y = GameCanvas.h - 35;
				}
				if (b50 == 1)
				{
					string p2 = msg.reader().readUTF();
					string caption2 = msg.reader().readUTF();
					ChatPopup.serverChatPopUp.cmdMsg1 = new Command(caption2, ChatPopup.serverChatPopUp, 1000, p2);
					ChatPopup.serverChatPopUp.cmdMsg1.x = GameCanvas.w / 2 - 75;
					ChatPopup.serverChatPopUp.cmdMsg1.y = GameCanvas.h - 35;
					ChatPopup.serverChatPopUp.cmdMsg2 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null);
					ChatPopup.serverChatPopUp.cmdMsg2.x = GameCanvas.w / 2 + 11;
					ChatPopup.serverChatPopUp.cmdMsg2.y = GameCanvas.h - 35;
				}
				break;
			}
			case 38:
			{
				GameCanvas.debug("SA67", 2);
				InfoDlg.hide();
				int num102 = msg.reader().readShort();
				Res.outz("OPEN_UI_SAY ID= " + num102);
				string str = msg.reader().readUTF();
				str = Res.changeString(str);
				for (int num132 = 0; num132 < GameScr.vNpc.size(); num132++)
				{
					Npc npc4 = (Npc)GameScr.vNpc.elementAt(num132);
					Res.outz("npc id= " + npc4.template.npcTemplateId);
					if (npc4.template.npcTemplateId == num102)
					{
						ChatPopup.addChatPopupMultiLine(str, 100000, npc4);
						GameCanvas.panel.hideNow();
						return;
					}
				}
				Npc npc5 = new Npc(num102, 0, 0, 0, num102, GameScr.info1.charId[Char.myCharz().cgender][2]);
				if (npc5.template.npcTemplateId == 5)
				{
					npc5.charID = 5;
				}
				try
				{
					npc5.avatar = msg.reader().readShort();
				}
				catch (Exception)
				{
				}
				ChatPopup.addChatPopupMultiLine(str, 100000, npc5);
				GameCanvas.panel.hideNow();
				break;
			}
			case 32:
			{
				GameCanvas.debug("SA68", 2);
				int num102 = msg.reader().readShort();
				for (int num103 = 0; num103 < GameScr.vNpc.size(); num103++)
				{
					Npc npc2 = (Npc)GameScr.vNpc.elementAt(num103);
					if (npc2.template.npcTemplateId == num102 && npc2.Equals(Char.myCharz().npcFocus))
					{
						string chat = msg.reader().readUTF();
						string[] array10 = new string[msg.reader().readByte()];
						for (int num104 = 0; num104 < array10.Length; num104++)
						{
							array10[num104] = msg.reader().readUTF();
						}
						GameScr.gI().createMenu(array10, npc2);
						ChatPopup.addChatPopup(chat, 100000, npc2);
						return;
					}
				}
				Npc npc3 = new Npc(num102, 0, -100, 100, num102, GameScr.info1.charId[Char.myCharz().cgender][2]);
				Res.outz((Char.myCharz().npcFocus == null) ? "null" : "!null");
				string chat2 = msg.reader().readUTF();
				string[] array11 = new string[msg.reader().readByte()];
				for (int num105 = 0; num105 < array11.Length; num105++)
				{
					array11[num105] = msg.reader().readUTF();
				}
				try
				{
					short num106 = (short)(npc3.avatar = msg.reader().readShort());
				}
				catch (Exception)
				{
				}
				Res.outz((Char.myCharz().npcFocus == null) ? "null" : "!null");
				GameScr.gI().createMenu(array11, npc3);
				ChatPopup.addChatPopup(chat2, 100000, npc3);
				break;
			}
			case 24:
			{
				GameCanvas.debug("SA69", 2);
				Char.myCharz().xuInBox = msg.reader().readInt();
				Char.myCharz().arrItemBox = new Item[msg.reader().readUnsignedByte()];
				for (int num91 = 0; num91 < Char.myCharz().arrItemBox.Length; num91++)
				{
					short num92 = msg.reader().readShort();
					if (num92 != -1)
					{
						Char.myCharz().arrItemBox[num91] = new Item();
						Char.myCharz().arrItemBox[num91].typeUI = 4;
						Char.myCharz().arrItemBox[num91].indexUI = num91;
						Char.myCharz().arrItemBox[num91].template = ItemTemplates.get(num92);
						Char.myCharz().arrItemBox[num91].isLock = msg.reader().readBool();
						if (Char.myCharz().arrItemBox[num91].isTypeBody())
						{
							Char.myCharz().arrItemBox[num91].upgrade = msg.reader().readByte();
						}
						Char.myCharz().arrItemBox[num91].isExpires = msg.reader().readBool();
						Char.myCharz().arrItemBox[num91].quantity = msg.reader().readShort();
					}
				}
				break;
			}
			case 7:
			{
				sbyte type = msg.reader().readByte();
				short id2 = msg.reader().readShort();
				string info4 = msg.reader().readUTF();
				GameCanvas.panel.saleRequest(type, info4, id2);
				break;
			}
			case 6:
				GameCanvas.debug("SA70", 2);
				Char.myCharz().xu = msg.reader().readInt();
				Char.myCharz().luong = msg.reader().readInt();
				Char.myCharz().luongKhoa = msg.reader().readInt();
				GameCanvas.endDlg();
				break;
			case -24:
				Char.isLoadingMap = true;
				Cout.println("GET MAP INFO");
				GameScr.gI().magicTree = null;
				GameCanvas.isLoading = true;
				GameCanvas.debug("SA75", 2);
				GameScr.resetAllvector();
				GameCanvas.endDlg();
				TileMap.vGo.removeAllElements();
				PopUp.vPopups.removeAllElements();
				mSystem.gcc();
				TileMap.mapID = msg.reader().readUnsignedByte();
				TileMap.planetID = msg.reader().readByte();
				TileMap.tileID = msg.reader().readByte();
				TileMap.bgID = msg.reader().readByte();
				Cout.println("load planet from server: " + TileMap.planetID + "bgType= " + TileMap.bgType + ".............................");
				TileMap.typeMap = msg.reader().readByte();
				TileMap.mapName = msg.reader().readUTF();
				TileMap.zoneID = msg.reader().readByte();
				GameCanvas.debug("SA75x1", 2);
				try
				{
					TileMap.loadMapFromResource(TileMap.mapID);
				}
				catch (Exception)
				{
					Service.gI().requestMaptemplate(TileMap.mapID);
					messWait = msg;
					return;
				}
				loadInfoMap(msg);
				try
				{
					sbyte b40 = msg.reader().readByte();
					TileMap.isMapDouble = ((b40 != 0) ? true : false);
				}
				catch (Exception)
				{
				}
				GameScr.cmx = GameScr.cmtoX;
				GameScr.cmy = GameScr.cmtoY;
				break;
			case -31:
			{
				if (GameCanvas.lowGraphic && TileMap.mapID != 51)
				{
					return;
				}
				TileMap.vItemBg.removeAllElements();
				short num79 = msg.reader().readShort();
				Cout.LogError2("nItem= " + num79);
				for (int num80 = 0; num80 < num79; num80++)
				{
					BgItem bgItem = new BgItem();
					bgItem.id = num80;
					bgItem.idImage = msg.reader().readShort();
					bgItem.layer = msg.reader().readByte();
					bgItem.dx = msg.reader().readShort();
					bgItem.dy = msg.reader().readShort();
					sbyte b37 = msg.reader().readByte();
					bgItem.tileX = new int[b37];
					bgItem.tileY = new int[b37];
					for (int num81 = 0; num81 < b37; num81++)
					{
						bgItem.tileX[num80] = msg.reader().readByte();
						bgItem.tileY[num80] = msg.reader().readByte();
					}
					TileMap.vItemBg.addElement(bgItem);
				}
				break;
			}
			case -4:
			{
				GameCanvas.debug("SA76", 2);
				@char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char == null)
				{
					return;
				}
				GameCanvas.debug("SA76v1", 2);
				if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
				{
					@char.setSkillPaint(GameScr.sks[msg.reader().readUnsignedByte()], 0);
				}
				else
				{
					@char.setSkillPaint(GameScr.sks[msg.reader().readUnsignedByte()], 1);
				}
				GameCanvas.debug("SA76v2", 2);
				@char.attMobs = new Mob[msg.reader().readByte()];
				for (int num48 = 0; num48 < @char.attMobs.Length; num48++)
				{
					Mob mob3 = (Mob)GameScr.vMob.elementAt(msg.reader().readByte());
					@char.attMobs[num48] = mob3;
					if (num48 == 0)
					{
						if (@char.cx <= mob3.x)
						{
							@char.cdir = 1;
						}
						else
						{
							@char.cdir = -1;
						}
					}
				}
				GameCanvas.debug("SA76v3", 2);
				@char.charFocus = null;
				@char.mobFocus = @char.attMobs[0];
				Char[] array = new Char[10];
				num = 0;
				try
				{
					for (num = 0; num < array.Length; num++)
					{
						int num13 = msg.reader().readInt();
						Char char7 = (array[num] = ((num13 != Char.myCharz().charID) ? GameScr.findCharInMap(num13) : Char.myCharz()));
						if (num == 0)
						{
							if (@char.cx <= char7.cx)
							{
								@char.cdir = 1;
							}
							else
							{
								@char.cdir = -1;
							}
						}
					}
				}
				catch (Exception ex6)
				{
					Cout.println("Loi PLAYER_ATTACK_N_P " + ex6.ToString());
				}
				GameCanvas.debug("SA76v4", 2);
				if (num > 0)
				{
					@char.attChars = new Char[num];
					for (num = 0; num < @char.attChars.Length; num++)
					{
						@char.attChars[num] = array[num];
					}
					@char.charFocus = @char.attChars[0];
					@char.mobFocus = null;
				}
				GameCanvas.debug("SA76v5", 2);
				break;
			}
			case 54:
			{
				@char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char == null)
				{
					return;
				}
				int num30 = msg.reader().readUnsignedByte();
				if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
				{
					@char.setSkillPaint(GameScr.sks[num30], 0);
				}
				else
				{
					@char.setSkillPaint(GameScr.sks[num30], 1);
				}
				GameCanvas.debug("SA769991v2", 2);
				Mob[] array6 = new Mob[10];
				num = 0;
				try
				{
					GameCanvas.debug("SA769991v3", 2);
					for (num = 0; num < array6.Length; num++)
					{
						GameCanvas.debug("SA769991v4-num" + num, 2);
						Mob mob2 = (array6[num] = (Mob)GameScr.vMob.elementAt(msg.reader().readByte()));
						if (num == 0)
						{
							if (@char.cx <= mob2.x)
							{
								@char.cdir = 1;
							}
							else
							{
								@char.cdir = -1;
							}
						}
						GameCanvas.debug("SA769991v5-num" + num, 2);
					}
				}
				catch (Exception ex5)
				{
					Cout.println("Loi PLAYER_ATTACK_NPC " + ex5.ToString());
				}
				GameCanvas.debug("SA769992", 2);
				if (num > 0)
				{
					@char.attMobs = new Mob[num];
					for (num = 0; num < @char.attMobs.Length; num++)
					{
						@char.attMobs[num] = array6[num];
					}
					@char.charFocus = null;
					@char.mobFocus = @char.attMobs[0];
				}
				break;
			}
			case -60:
			{
				GameCanvas.debug("SA7666", 2);
				int num2 = msg.reader().readInt();
				int num3 = -1;
				if (num2 != Char.myCharz().charID)
				{
					Char char2 = GameScr.findCharInMap(num2);
					if (char2 == null)
					{
						return;
					}
					if (char2.currentMovePoint != null)
					{
						char2.createShadow(char2.cx, char2.cy, 10);
						char2.cx = char2.currentMovePoint.xEnd;
						char2.cy = char2.currentMovePoint.yEnd;
					}
					int num4 = msg.reader().readUnsignedByte();
					Res.outz("player skill ID= " + num4);
					if ((TileMap.tileTypeAtPixel(char2.cx, char2.cy) & 2) == 2)
					{
						char2.setSkillPaint(GameScr.sks[num4], 0);
					}
					else
					{
						char2.setSkillPaint(GameScr.sks[num4], 1);
					}
					sbyte b = msg.reader().readByte();
					Res.outz("nAttack = " + b);
					Char[] array = new Char[b];
					for (num = 0; num < array.Length; num++)
					{
						num3 = msg.reader().readInt();
						Char char3;
						if (num3 == Char.myCharz().charID)
						{
							char3 = Char.myCharz();
							if (!GameScr.isChangeZone && GameScr.isAutoPlay && GameScr.canAutoPlay)
							{
								Service.gI().requestChangeZone(-1, -1);
								GameScr.isChangeZone = true;
							}
						}
						else
						{
							char3 = GameScr.findCharInMap(num3);
						}
						array[num] = char3;
						if (num == 0)
						{
							if (char2.cx <= char3.cx)
							{
								char2.cdir = 1;
							}
							else
							{
								char2.cdir = -1;
							}
						}
					}
					if (num > 0)
					{
						char2.attChars = new Char[num];
						for (num = 0; num < char2.attChars.Length; num++)
						{
							char2.attChars[num] = array[num];
						}
						char2.mobFocus = null;
						char2.charFocus = char2.attChars[0];
					}
				}
				else
				{
					sbyte b2 = msg.reader().readByte();
					sbyte b3 = msg.reader().readByte();
					num3 = msg.reader().readInt();
				}
				sbyte b4 = msg.reader().readByte();
				Res.outz("isRead continue = " + b4);
				if (b4 != 1)
				{
					break;
				}
				sbyte b5 = msg.reader().readByte();
				Res.outz("type skill = " + b5);
				if (num3 == Char.myCharz().charID)
				{
					bool flag = false;
					@char = Char.myCharz();
					int num5 = msg.readInt3Byte();
					Res.outz("dame hit = " + num5);
					@char.isDie = msg.reader().readBoolean();
					if (@char.isDie)
					{
						Char.isLockKey = true;
					}
					Res.outz("isDie=" + @char.isDie + "---------------------------------------");
					int num6 = 0;
					flag = (@char.isCrit = msg.reader().readBoolean());
					@char.isMob = false;
					num5 = (@char.damHP = num5 + num6);
					if (b5 == 0)
					{
						@char.doInjure(num5, 0, flag, isMob: false);
					}
				}
				else
				{
					@char = GameScr.findCharInMap(num3);
					if (@char == null)
					{
						return;
					}
					bool flag2 = false;
					int num7 = msg.readInt3Byte();
					Res.outz("dame hit= " + num7);
					@char.isDie = msg.reader().readBoolean();
					Res.outz("isDie=" + @char.isDie + "---------------------------------------");
					int num8 = 0;
					flag2 = (@char.isCrit = msg.reader().readBoolean());
					@char.isMob = false;
					num7 = (@char.damHP = num7 + num8);
					if (b5 == 0)
					{
						@char.doInjure(num7, 0, flag2, isMob: false);
					}
				}
				break;
			}
			}
			switch (msg.command)
			{
			case -2:
			{
				GameCanvas.debug("SA77", 22);
				int num183 = msg.reader().readInt();
				Char.myCharz().yen += num183;
				GameScr.startFlyText((num183 <= 0) ? (string.Empty + num183) : ("+" + num183), Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
				break;
			}
			case 95:
			{
				GameCanvas.debug("SA77", 22);
				int num190 = msg.reader().readInt();
				Char.myCharz().xu += num190;
				GameScr.startFlyText((num190 <= 0) ? (string.Empty + num190) : ("+" + num190), Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
				break;
			}
			case 96:
				GameCanvas.debug("SA77a", 22);
				Char.myCharz().taskOrders.addElement(new TaskOrder(msg.reader().readByte(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readUTF(), msg.reader().readUTF(), msg.reader().readByte(), msg.reader().readByte()));
				break;
			case 97:
			{
				sbyte b71 = msg.reader().readByte();
				for (int num176 = 0; num176 < Char.myCharz().taskOrders.size(); num176++)
				{
					TaskOrder taskOrder = (TaskOrder)Char.myCharz().taskOrders.elementAt(num176);
					if (taskOrder.taskId == b71)
					{
						taskOrder.count = msg.reader().readShort();
						break;
					}
				}
				break;
			}
			case -1:
			{
				GameCanvas.debug("SA77", 222);
				int num173 = msg.reader().readInt();
				Char.myCharz().xu += num173;
				Char.myCharz().yen -= num173;
				GameScr.startFlyText("+" + num173, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
				break;
			}
			case -3:
			{
				GameCanvas.debug("SA78", 2);
				sbyte b77 = msg.reader().readByte();
				int num185 = msg.reader().readInt();
				if (b77 == 0)
				{
					Char.myCharz().cPower += num185;
				}
				if (b77 == 1)
				{
					Char.myCharz().cTiemNang += num185;
				}
				if (b77 == 2)
				{
					Char.myCharz().cPower += num185;
					Char.myCharz().cTiemNang += num185;
				}
				Char.myCharz().applyCharLevelPercent();
				if (Char.myCharz().cTypePk != 3)
				{
					GameScr.startFlyText(((num185 <= 0) ? string.Empty : "+") + num185, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -4, mFont.GREEN);
					if (num185 > 0 && Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5002)
					{
						ServerEffect.addServerEffect(55, Char.myCharz().petFollow.cmx, Char.myCharz().petFollow.cmy, 1);
						ServerEffect.addServerEffect(55, Char.myCharz().cx, Char.myCharz().cy, 1);
					}
				}
				break;
			}
			case -73:
			{
				sbyte b75 = msg.reader().readByte();
				for (int num184 = 0; num184 < GameScr.vNpc.size(); num184++)
				{
					Npc npc7 = (Npc)GameScr.vNpc.elementAt(num184);
					if (npc7.template.npcTemplateId == b75)
					{
						sbyte b76 = msg.reader().readByte();
						if (b76 == 0)
						{
							npc7.isHide = true;
						}
						else
						{
							npc7.isHide = false;
						}
						break;
					}
				}
				break;
			}
			case -5:
			{
				GameCanvas.debug("SA79", 2);
				int charID = msg.reader().readInt();
				int num178 = msg.reader().readInt();
				Char char14;
				if (num178 != -100)
				{
					char14 = new Char();
					char14.charID = charID;
					char14.clanID = num178;
				}
				else
				{
					char14 = new Mabu();
					char14.charID = charID;
					char14.clanID = num178;
				}
				if (char14.clanID == -2)
				{
					char14.isCopy = true;
				}
				if (readCharInfo(char14, msg))
				{
					sbyte b73 = msg.reader().readByte();
					if (char14.cy <= 10 && b73 != 0 && b73 != 2)
					{
						Res.outz("nhân vật bay trên trời xuống x= " + char14.cx + " y= " + char14.cy);
						Teleport teleport2 = new Teleport(char14.cx, char14.cy, char14.head, char14.cdir, 1, isMe: false, (b73 != 1) ? b73 : char14.cgender);
						teleport2.id = char14.charID;
						char14.isTeleport = true;
						Teleport.addTeleport(teleport2);
					}
					if (b73 == 2)
					{
						char14.show();
					}
					for (int num179 = 0; num179 < GameScr.vMob.size(); num179++)
					{
						Mob mob10 = (Mob)GameScr.vMob.elementAt(num179);
						if (mob10 != null && mob10.isMobMe && mob10.mobId == char14.charID)
						{
							Res.outz("co 1 con quai");
							char14.mobMe = mob10;
							char14.mobMe.x = char14.cx;
							char14.mobMe.y = char14.cy - 40;
							break;
						}
					}
					if (GameScr.findCharInMap(char14.charID) == null)
					{
						GameScr.vCharInMap.addElement(char14);
					}
					char14.isMonkey = msg.reader().readByte();
					short num180 = msg.reader().readShort();
					Res.outz("mount id= " + num180 + "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
					if (num180 != -1)
					{
						char14.isHaveMount = true;
						switch (num180)
						{
						case 346:
						case 347:
						case 348:
							char14.isMountVip = false;
							break;
						case 349:
						case 350:
						case 351:
							char14.isMountVip = true;
							break;
						case 396:
							char14.isEventMount = true;
							break;
						case 532:
							char14.isSpeacialMount = true;
							break;
						default:
							if (num180 >= Char.ID_NEW_MOUNT)
							{
								char14.idMount = num180;
							}
							break;
						}
					}
					else
					{
						char14.isHaveMount = false;
					}
				}
				sbyte b74 = msg.reader().readByte();
				Res.outz("addplayer:   " + b74);
				char14.cFlag = b74;
				char14.isNhapThe = msg.reader().readByte() == 1;
				GameScr.gI().getFlagImage(char14.charID, char14.cFlag);
				break;
			}
			case -7:
			{
				GameCanvas.debug("SA80", 2);
				int num174 = msg.reader().readInt();
				Cout.println("RECEVED MOVE OF " + num174);
				for (int num175 = 0; num175 < GameScr.vCharInMap.size(); num175++)
				{
					Char char13 = null;
					try
					{
						char13 = (Char)GameScr.vCharInMap.elementAt(num175);
					}
					catch (Exception ex23)
					{
						Cout.println("Loi PLAYER_MOVE " + ex23.ToString());
					}
					if (char13 == null)
					{
						break;
					}
					if (char13.charID == num174)
					{
						GameCanvas.debug("SA8x2y" + num175, 2);
						char13.moveTo(msg.reader().readShort(), msg.reader().readShort(), 0);
						char13.lastUpdateTime = mSystem.currentTimeMillis();
						break;
					}
				}
				GameCanvas.debug("SA80x3", 2);
				break;
			}
			case -6:
			{
				GameCanvas.debug("SA81", 2);
				int num174 = msg.reader().readInt();
				for (int num189 = 0; num189 < GameScr.vCharInMap.size(); num189++)
				{
					Char char15 = (Char)GameScr.vCharInMap.elementAt(num189);
					if (char15 != null && char15.charID == num174)
					{
						if (!char15.isInvisiblez && !char15.isUsePlane)
						{
							ServerEffect.addServerEffect(60, char15.cx, char15.cy, 1);
						}
						if (!char15.isUsePlane)
						{
							GameScr.vCharInMap.removeElementAt(num189);
						}
						return;
					}
				}
				break;
			}
			case -13:
			{
				GameCanvas.debug("SA82", 2);
				int num181 = msg.reader().readUnsignedByte();
				if (num181 > GameScr.vMob.size() - 1 || num181 < 0)
				{
					return;
				}
				Mob mob9 = (Mob)GameScr.vMob.elementAt(num181);
				mob9.sys = msg.reader().readByte();
				mob9.levelBoss = msg.reader().readByte();
				if (mob9.levelBoss != 0)
				{
					mob9.typeSuperEff = Res.random(0, 3);
				}
				mob9.x = mob9.xFirst;
				mob9.y = mob9.yFirst;
				mob9.status = 5;
				mob9.injureThenDie = false;
				mob9.hp = msg.reader().readInt();
				mob9.maxHp = mob9.hp;
				ServerEffect.addServerEffect(60, mob9.x, mob9.y, 1);
				break;
			}
			case -75:
			{
				Mob mob9 = null;
				try
				{
					mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				if (mob9 != null)
				{
					mob9.levelBoss = msg.reader().readByte();
					if (mob9.levelBoss > 0)
					{
						mob9.typeSuperEff = Res.random(0, 3);
					}
				}
				break;
			}
			case -9:
			{
				GameCanvas.debug("SA83", 2);
				Mob mob9 = null;
				try
				{
					mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				GameCanvas.debug("SA83v1", 2);
				if (mob9 != null)
				{
					mob9.hp = msg.readInt3Byte();
					int num177 = msg.readInt3Byte();
					if (num177 == 1)
					{
						return;
					}
					bool flag11 = false;
					try
					{
						flag11 = msg.reader().readBoolean();
					}
					catch (Exception)
					{
					}
					sbyte b72 = msg.reader().readByte();
					if (b72 != -1)
					{
						EffecMn.addEff(new Effect(b72, mob9.x, mob9.getY(), 3, 1, -1));
					}
					GameCanvas.debug("SA83v2", 2);
					if (flag11)
					{
						GameScr.startFlyText("-" + num177, mob9.x, mob9.getY() - mob9.getH(), 0, -2, mFont.FATAL);
					}
					else if (num177 == 0)
					{
						mob9.x = mob9.xFirst;
						mob9.y = mob9.yFirst;
						GameScr.startFlyText(mResources.miss, mob9.x, mob9.getY() - mob9.getH(), 0, -2, mFont.MISS);
					}
					else
					{
						GameScr.startFlyText("-" + num177, mob9.x, mob9.getY() - mob9.getH(), 0, -2, mFont.ORANGE);
					}
				}
				GameCanvas.debug("SA83v3", 2);
				break;
			}
			case 45:
			{
				GameCanvas.debug("SA84", 2);
				Mob mob9 = null;
				try
				{
					mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception ex22)
				{
					Cout.println("Loi tai NPC_MISS  " + ex22.ToString());
				}
				if (mob9 != null)
				{
					mob9.hp = msg.reader().readInt();
					GameScr.startFlyText(mResources.miss, mob9.x, mob9.y - mob9.h, 0, -2, mFont.MISS);
				}
				break;
			}
			case -12:
			{
				Res.outz("SERVER SEND MOB DIE");
				GameCanvas.debug("SA85", 2);
				Mob mob9 = null;
				try
				{
					mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
					Cout.println("LOi tai NPC_DIE cmd " + msg.command);
				}
				if (mob9 == null || mob9.status == 0 || mob9.status == 0)
				{
					break;
				}
				mob9.startDie();
				try
				{
					int num186 = msg.readInt3Byte();
					if (msg.reader().readBool())
					{
						GameScr.startFlyText("-" + num186, mob9.x, mob9.y - mob9.h, 0, -2, mFont.FATAL);
					}
					else
					{
						GameScr.startFlyText("-" + num186, mob9.x, mob9.y - mob9.h, 0, -2, mFont.ORANGE);
					}
					sbyte b78 = msg.reader().readByte();
					for (int num187 = 0; num187 < b78; num187++)
					{
						ItemMap itemMap4 = new ItemMap(msg.reader().readShort(), msg.reader().readShort(), mob9.x, mob9.y, msg.reader().readShort(), msg.reader().readShort());
						int num188 = (itemMap4.playerId = msg.reader().readInt());
						Res.outz("playerid= " + num188 + " my id= " + Char.myCharz().charID);
						GameScr.vItemMap.addElement(itemMap4);
						if (Res.abs(itemMap4.y - Char.myCharz().cy) < 24 && Res.abs(itemMap4.x - Char.myCharz().cx) < 24)
						{
							Char.myCharz().charFocus = null;
						}
					}
				}
				catch (Exception ex32)
				{
					Cout.println("LOi tai NPC_DIE " + ex32.ToString() + " cmd " + msg.command);
				}
				break;
			}
			case 74:
			{
				GameCanvas.debug("SA85", 2);
				Mob mob9 = null;
				try
				{
					mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
					Cout.println("Loi tai NPC CHANGE " + msg.command);
				}
				if (mob9 != null && mob9.status != 0 && mob9.status != 0)
				{
					mob9.status = 0;
					ServerEffect.addServerEffect(60, mob9.x, mob9.y, 1);
					ItemMap itemMap3 = new ItemMap(msg.reader().readShort(), msg.reader().readShort(), mob9.x, mob9.y, msg.reader().readShort(), msg.reader().readShort());
					GameScr.vItemMap.addElement(itemMap3);
					if (Res.abs(itemMap3.y - Char.myCharz().cy) < 24 && Res.abs(itemMap3.x - Char.myCharz().cx) < 24)
					{
						Char.myCharz().charFocus = null;
					}
				}
				break;
			}
			case -11:
			{
				GameCanvas.debug("SA86", 2);
				Mob mob9 = null;
				try
				{
					int index4 = msg.reader().readUnsignedByte();
					mob9 = (Mob)GameScr.vMob.elementAt(index4);
				}
				catch (Exception)
				{
					Cout.println("Loi tai NPC_ATTACK_ME " + msg.command);
				}
				if (mob9 != null)
				{
					Char.myCharz().isDie = false;
					Char.isLockKey = false;
					int num170 = msg.readInt3Byte();
					int num171;
					try
					{
						num171 = msg.readInt3Byte();
					}
					catch (Exception)
					{
						num171 = 0;
					}
					if (mob9.isBusyAttackSomeOne)
					{
						Char.myCharz().doInjure(num170, num171, isCrit: false, isMob: true);
						break;
					}
					mob9.dame = num170;
					mob9.dameMp = num171;
					mob9.setAttack(Char.myCharz());
				}
				break;
			}
			case -10:
			{
				GameCanvas.debug("SA87", 2);
				Mob mob9 = null;
				try
				{
					mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				GameCanvas.debug("SA87x1", 2);
				if (mob9 != null)
				{
					GameCanvas.debug("SA87x2", 2);
					@char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char == null)
					{
						return;
					}
					GameCanvas.debug("SA87x3", 2);
					int num182 = msg.readInt3Byte();
					mob9.dame = @char.cHP - num182;
					@char.cHPNew = num182;
					GameCanvas.debug("SA87x4", 2);
					try
					{
						@char.cMP = msg.readInt3Byte();
					}
					catch (Exception)
					{
					}
					GameCanvas.debug("SA87x5", 2);
					if (mob9.isBusyAttackSomeOne)
					{
						@char.doInjure(mob9.dame, 0, isCrit: false, isMob: true);
					}
					else
					{
						mob9.setAttack(@char);
					}
					GameCanvas.debug("SA87x6", 2);
				}
				break;
			}
			case -17:
				GameCanvas.debug("SA88", 2);
				Char.myCharz().meDead = true;
				Char.myCharz().cPk = msg.reader().readByte();
				Char.myCharz().startDie(msg.reader().readShort(), msg.reader().readShort());
				try
				{
					Char.myCharz().cPower = msg.reader().readLong();
					Char.myCharz().applyCharLevelPercent();
				}
				catch (Exception)
				{
					Cout.println("Loi tai ME_DIE " + msg.command);
				}
				Char.myCharz().countKill = 0;
				break;
			case 66:
				Res.outz("ME DIE XP DOWN NOT IMPLEMENT YET!!!!!!!!!!!!!!!!!!!!!!!!!!");
				break;
			case -8:
				GameCanvas.debug("SA89", 2);
				@char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char == null)
				{
					return;
				}
				@char.cPk = msg.reader().readByte();
				@char.waitToDie(msg.reader().readShort(), msg.reader().readShort());
				break;
			case -16:
				GameCanvas.debug("SA90", 2);
				if (Char.myCharz().wdx != 0 || Char.myCharz().wdy != 0)
				{
					Char.myCharz().cx = Char.myCharz().wdx;
					Char.myCharz().cy = Char.myCharz().wdy;
					Char.myCharz().wdx = (Char.myCharz().wdy = 0);
				}
				Char.myCharz().liveFromDead();
				Char.myCharz().isLockMove = false;
				Char.myCharz().meDead = false;
				break;
			case 44:
			{
				GameCanvas.debug("SA91", 2);
				int num172 = msg.reader().readInt();
				string text8 = msg.reader().readUTF();
				Res.outz("user id= " + num172 + " text= " + text8);
				@char = ((Char.myCharz().charID != num172) ? GameScr.findCharInMap(num172) : Char.myCharz());
				if (@char == null)
				{
					return;
				}
				@char.addInfo(text8);
				break;
			}
			case 18:
			{
				sbyte b70 = msg.reader().readByte();
				for (int num169 = 0; num169 < b70; num169++)
				{
					int charId = msg.reader().readInt();
					int cx = msg.reader().readShort();
					int cy = msg.reader().readShort();
					int cHPShow = msg.readInt3Byte();
					Char char12 = GameScr.findCharInMap(charId);
					if (char12 != null)
					{
						char12.cx = cx;
						char12.cy = cy;
						char12.cHP = (char12.cHPShow = cHPShow);
						char12.lastUpdateTime = mSystem.currentTimeMillis();
					}
				}
				break;
			}
			case 19:
				Char.myCharz().countKill = msg.reader().readUnsignedShort();
				Char.myCharz().countKillMax = msg.reader().readUnsignedShort();
				break;
			}
			GameCanvas.debug("SA92", 2);
		}
		catch (Exception)
		{
		}
		finally
		{
			msg?.cleanup();
		}
	}

	private void createItem(myReader d)
	{
		GameScr.vcItem = d.readByte();
		ItemTemplates.itemTemplates.clear();
		GameScr.gI().iOptionTemplates = new ItemOptionTemplate[d.readUnsignedByte()];
		for (int i = 0; i < GameScr.gI().iOptionTemplates.Length; i++)
		{
			GameScr.gI().iOptionTemplates[i] = new ItemOptionTemplate();
			GameScr.gI().iOptionTemplates[i].id = i;
			GameScr.gI().iOptionTemplates[i].name = d.readUTF();
			GameScr.gI().iOptionTemplates[i].type = d.readByte();
		}
		int num = d.readShort();
		for (int j = 0; j < num; j++)
		{
			ItemTemplate it = new ItemTemplate((short)j, d.readByte(), d.readByte(), d.readUTF(), d.readUTF(), d.readByte(), d.readInt(), d.readShort(), d.readShort(), d.readBool());
			ItemTemplates.add(it);
		}
	}

	private void createSkill(myReader d)
	{
		GameScr.vcSkill = d.readByte();
		GameScr.gI().sOptionTemplates = new SkillOptionTemplate[d.readByte()];
		for (int i = 0; i < GameScr.gI().sOptionTemplates.Length; i++)
		{
			GameScr.gI().sOptionTemplates[i] = new SkillOptionTemplate();
			GameScr.gI().sOptionTemplates[i].id = i;
			GameScr.gI().sOptionTemplates[i].name = d.readUTF();
		}
		GameScr.nClasss = new NClass[d.readByte()];
		for (int j = 0; j < GameScr.nClasss.Length; j++)
		{
			GameScr.nClasss[j] = new NClass();
			GameScr.nClasss[j].classId = j;
			GameScr.nClasss[j].name = d.readUTF();
			GameScr.nClasss[j].skillTemplates = new SkillTemplate[d.readByte()];
			for (int k = 0; k < GameScr.nClasss[j].skillTemplates.Length; k++)
			{
				GameScr.nClasss[j].skillTemplates[k] = new SkillTemplate();
				GameScr.nClasss[j].skillTemplates[k].id = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].name = d.readUTF();
				GameScr.nClasss[j].skillTemplates[k].maxPoint = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].manaUseType = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].type = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].iconId = d.readShort();
				GameScr.nClasss[j].skillTemplates[k].damInfo = d.readUTF();
				int lineWidth = 130;
				if (GameCanvas.w == 128 || GameCanvas.h <= 208)
				{
					lineWidth = 100;
				}
				GameScr.nClasss[j].skillTemplates[k].description = mFont.tahoma_7_green2.splitFontArray(d.readUTF(), lineWidth);
				GameScr.nClasss[j].skillTemplates[k].skills = new Skill[d.readByte()];
				for (int l = 0; l < GameScr.nClasss[j].skillTemplates[k].skills.Length; l++)
				{
					GameScr.nClasss[j].skillTemplates[k].skills[l] = new Skill();
					GameScr.nClasss[j].skillTemplates[k].skills[l].skillId = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].template = GameScr.nClasss[j].skillTemplates[k];
					GameScr.nClasss[j].skillTemplates[k].skills[l].point = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].skills[l].powRequire = d.readLong();
					GameScr.nClasss[j].skillTemplates[k].skills[l].manaUse = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].coolDown = d.readInt();
					GameScr.nClasss[j].skillTemplates[k].skills[l].dx = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].dy = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].maxFight = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].skills[l].damage = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].price = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].moreInfo = d.readUTF();
					Skills.add(GameScr.nClasss[j].skillTemplates[k].skills[l]);
				}
			}
		}
	}

	private void createMap(myReader d)
	{
		GameScr.vcMap = d.readByte();
		TileMap.mapNames = new string[d.readUnsignedByte()];
		for (int i = 0; i < TileMap.mapNames.Length; i++)
		{
			TileMap.mapNames[i] = d.readUTF();
		}
		Npc.arrNpcTemplate = new NpcTemplate[d.readByte()];
		for (sbyte b = 0; b < Npc.arrNpcTemplate.Length; b = (sbyte)(b + 1))
		{
			Npc.arrNpcTemplate[b] = new NpcTemplate();
			Npc.arrNpcTemplate[b].npcTemplateId = b;
			Npc.arrNpcTemplate[b].name = d.readUTF();
			Npc.arrNpcTemplate[b].headId = d.readShort();
			Npc.arrNpcTemplate[b].bodyId = d.readShort();
			Npc.arrNpcTemplate[b].legId = d.readShort();
			Npc.arrNpcTemplate[b].menu = new string[d.readByte()][];
			for (int j = 0; j < Npc.arrNpcTemplate[b].menu.Length; j++)
			{
				Npc.arrNpcTemplate[b].menu[j] = new string[d.readByte()];
				for (int k = 0; k < Npc.arrNpcTemplate[b].menu[j].Length; k++)
				{
					Npc.arrNpcTemplate[b].menu[j][k] = d.readUTF();
				}
			}
		}
		Mob.arrMobTemplate = new MobTemplate[d.readByte()];
		for (sbyte b2 = 0; b2 < Mob.arrMobTemplate.Length; b2 = (sbyte)(b2 + 1))
		{
			Mob.arrMobTemplate[b2] = new MobTemplate();
			Mob.arrMobTemplate[b2].mobTemplateId = b2;
			Mob.arrMobTemplate[b2].type = d.readByte();
			Mob.arrMobTemplate[b2].name = d.readUTF();
			Mob.arrMobTemplate[b2].hp = d.readInt();
			Mob.arrMobTemplate[b2].rangeMove = d.readByte();
			Mob.arrMobTemplate[b2].speed = d.readByte();
			Mob.arrMobTemplate[b2].dartType = d.readByte();
		}
	}

	private void createData(myReader d, bool isSaveRMS)
	{
		GameScr.vcData = d.readByte();
		if (isSaveRMS)
		{
			Rms.saveRMS("NR_dart", NinjaUtil.readByteArray(d));
			Rms.saveRMS("NR_arrow", NinjaUtil.readByteArray(d));
			Rms.saveRMS("NR_effect", NinjaUtil.readByteArray(d));
			Rms.saveRMS("NR_image", NinjaUtil.readByteArray(d));
			Rms.saveRMS("NR_part", NinjaUtil.readByteArray(d));
			Rms.saveRMS("NR_skill", NinjaUtil.readByteArray(d));
			Rms.DeleteStorage("NRdata");
		}
	}

	private Image createImage(sbyte[] arr)
	{
		try
		{
			return Image.createImage(arr, 0, arr.Length);
		}
		catch (Exception)
		{
		}
		return null;
	}

	public int[] arrayByte2Int(sbyte[] b)
	{
		int[] array = new int[b.Length];
		for (int i = 0; i < b.Length; i++)
		{
			int num = b[i];
			if (num < 0)
			{
				num += 256;
			}
			array[i] = num;
		}
		return array;
	}

	public void readClanMsg(Message msg, int index)
	{
		try
		{
			ClanMessage clanMessage = new ClanMessage();
			sbyte b = msg.reader().readByte();
			clanMessage.type = b;
			clanMessage.id = msg.reader().readInt();
			clanMessage.playerId = msg.reader().readInt();
			clanMessage.playerName = msg.reader().readUTF();
			clanMessage.role = msg.reader().readByte();
			clanMessage.time = msg.reader().readInt() + 1000000000;
			bool flag = false;
			GameScr.isNewClanMessage = false;
			if (b == 0)
			{
				string text = msg.reader().readUTF();
				GameScr.isNewClanMessage = true;
				if (mFont.tahoma_7.getWidth(text) > Panel.WIDTH_PANEL - 60)
				{
					clanMessage.chat = mFont.tahoma_7.splitFontArray(text, Panel.WIDTH_PANEL - 10);
				}
				else
				{
					clanMessage.chat = new string[1];
					clanMessage.chat[0] = text;
				}
				clanMessage.color = msg.reader().readByte();
			}
			else if (b == 1)
			{
				clanMessage.recieve = msg.reader().readByte();
				clanMessage.maxCap = msg.reader().readByte();
				flag = msg.reader().readByte() == 1;
				if (flag)
				{
					GameScr.isNewClanMessage = true;
				}
				if (clanMessage.playerId != Char.myCharz().charID)
				{
					if (clanMessage.recieve < clanMessage.maxCap)
					{
						clanMessage.option = new string[1] { mResources.donate };
					}
					else
					{
						clanMessage.option = null;
					}
				}
				if (GameCanvas.panel.cp != null)
				{
					GameCanvas.panel.updateRequest(clanMessage.recieve, clanMessage.maxCap);
				}
			}
			else if (b == 2 && Char.myCharz().role == 0)
			{
				GameScr.isNewClanMessage = true;
				clanMessage.option = new string[2]
				{
					mResources.CANCEL,
					mResources.receive
				};
			}
			if (GameCanvas.currentScreen != GameScr.instance)
			{
				GameScr.isNewClanMessage = false;
			}
			else if (GameCanvas.panel.isShow && GameCanvas.panel.type == 0 && GameCanvas.panel.currentTabIndex == 3)
			{
				GameScr.isNewClanMessage = false;
			}
			ClanMessage.addMessage(clanMessage, index, flag);
		}
		catch (Exception)
		{
			Cout.println("LOI TAI CMD -= " + msg.command);
		}
	}

	public void loadCurrMap(sbyte teleport3)
	{
		Res.outz("is loading map = " + Char.isLoadingMap);
		GameScr.gI().auto = 0;
		GameScr.isChangeZone = false;
		CreateCharScr.instance = null;
		GameScr.info1.isUpdate = false;
		GameScr.info2.isUpdate = false;
		GameScr.lockTick = 0;
		GameCanvas.panel.isShow = false;
		SoundMn.gI().stopAll();
		if (!GameScr.isLoadAllData && !CreateCharScr.isCreateChar)
		{
			GameScr.gI().initSelectChar();
		}
		GameScr.loadCamera(fullmScreen: false, (teleport3 != 1) ? (-1) : Char.myCharz().cx, (teleport3 == 0) ? (-1) : 0);
		TileMap.loadMainTile();
		TileMap.loadMap(TileMap.tileID);
		Res.outz("LOAD GAMESCR 2");
		Char.myCharz().cvx = 0;
		Char.myCharz().statusMe = 4;
		Char.myCharz().currentMovePoint = null;
		Char.myCharz().mobFocus = null;
		Char.myCharz().charFocus = null;
		Char.myCharz().npcFocus = null;
		Char.myCharz().itemFocus = null;
		Char.myCharz().skillPaint = null;
		Char.myCharz().setMabuHold(m: false);
		Char.myCharz().skillPaintRandomPaint = null;
		GameCanvas.clearAllPointerEvent();
		if (Char.myCharz().cy >= TileMap.pxh - 100)
		{
			Char.myCharz().isFlyUp = true;
			Char.myCharz().cx += Res.abs(Res.random(0, 80));
			Service.gI().charMove();
		}
		GameScr.gI().loadGameScr();
		GameCanvas.loadBG(TileMap.bgID);
		Char.isLockKey = false;
		Res.outz("cy= " + Char.myCharz().cy + "---------------------------------------------");
		for (int i = 0; i < Char.myCharz().vEff.size(); i++)
		{
			EffectChar effectChar = (EffectChar)Char.myCharz().vEff.elementAt(i);
			if (effectChar.template.type == 10)
			{
				Char.isLockKey = true;
				break;
			}
		}
		GameCanvas.clearKeyHold();
		GameCanvas.clearKeyPressed();
		GameScr.gI().dHP = Char.myCharz().cHP;
		GameScr.gI().dMP = Char.myCharz().cMP;
		Char.ischangingMap = false;
		GameScr.gI().switchToMe();
		if (Char.myCharz().cy <= 10 && teleport3 != 0 && teleport3 != 2)
		{
			Teleport p = new Teleport(Char.myCharz().cx, Char.myCharz().cy, Char.myCharz().head, Char.myCharz().cdir, 1, isMe: true, (teleport3 != 1) ? teleport3 : Char.myCharz().cgender);
			Teleport.addTeleport(p);
			Char.myCharz().isTeleport = true;
		}
		if (teleport3 == 2)
		{
			Char.myCharz().show();
		}
		if (GameScr.gI().isRongThanXuatHien)
		{
			if (TileMap.mapID == GameScr.gI().mapRID && TileMap.zoneID == GameScr.gI().zoneRID)
			{
				GameScr.gI().callRongThan(GameScr.gI().xR, GameScr.gI().yR);
			}
			if (mGraphics.zoomLevel > 1)
			{
				GameScr.gI().doiMauTroi();
			}
		}
		InfoDlg.hide();
		InfoDlg.show(TileMap.mapName, mResources.zone + " " + TileMap.zoneID, 30);
		GameCanvas.endDlg();
		GameCanvas.isLoading = false;
		Hint.clickMob();
		Hint.clickNpc();
		GameCanvas.debug("SA75x9", 2);
	}

	public void loadInfoMap(Message msg)
	{
		try
		{
			if (mGraphics.zoomLevel == 1)
			{
				SmallImage.clearHastable();
			}
			Char.myCharz().cx = (Char.myCharz().cxSend = (Char.myCharz().cxFocus = msg.reader().readShort()));
			Char.myCharz().cy = (Char.myCharz().cySend = (Char.myCharz().cyFocus = msg.reader().readShort()));
			Char.myCharz().xSd = Char.myCharz().cx;
			Char.myCharz().ySd = Char.myCharz().cy;
			Res.outz("head= " + Char.myCharz().head + " body= " + Char.myCharz().body + " left= " + Char.myCharz().leg + " x= " + Char.myCharz().cx + " y= " + Char.myCharz().cy + " chung toc= " + Char.myCharz().cgender);
			if (Char.myCharz().cx >= 0 && Char.myCharz().cx <= 100)
			{
				Char.myCharz().cdir = 1;
			}
			else if (Char.myCharz().cx >= TileMap.tmw - 100 && Char.myCharz().cx <= TileMap.tmw)
			{
				Char.myCharz().cdir = -1;
			}
			GameCanvas.debug("SA75x4", 2);
			int num = msg.reader().readByte();
			Res.outz("vGo size= " + num);
			if (!GameScr.info1.isDone)
			{
				GameScr.info1.cmx = Char.myCharz().cx - GameScr.cmx;
				GameScr.info1.cmy = Char.myCharz().cy - GameScr.cmy;
			}
			for (int i = 0; i < num; i++)
			{
				Waypoint waypoint = new Waypoint(msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readUTF());
				if ((TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23) || waypoint.minX < 0 || waypoint.minX <= 24)
				{
				}
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
			GameCanvas.debug("SA75x5", 2);
			num = msg.reader().readByte();
			Mob.newMob.removeAllElements();
			for (sbyte b = 0; b < num; b = (sbyte)(b + 1))
			{
				Mob mob = new Mob(b, msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readByte(), msg.reader().readByte(), msg.reader().readInt(), msg.reader().readByte(), msg.reader().readInt(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readByte(), msg.reader().readByte());
				mob.xSd = mob.x;
				mob.ySd = mob.y;
				mob.isBoss = msg.reader().readBoolean();
				if (Mob.arrMobTemplate[mob.templateId].type != 0)
				{
					if (b % 3 == 0)
					{
						mob.dir = -1;
					}
					else
					{
						mob.dir = 1;
					}
					mob.x += 10 - b % 20;
				}
				mob.isMobMe = false;
				BigBoss bigBoss = null;
				BachTuoc bachTuoc = null;
				BigBoss2 bigBoss2 = null;
				NewBoss newBoss = null;
				if (mob.templateId == 70)
				{
					bigBoss = new BigBoss(b, (short)mob.x, (short)mob.y, 70, mob.hp, mob.maxHp, mob.sys);
				}
				if (mob.templateId == 71)
				{
					bachTuoc = new BachTuoc(b, (short)mob.x, (short)mob.y, 71, mob.hp, mob.maxHp, mob.sys);
				}
				if (mob.templateId == 72)
				{
					bigBoss2 = new BigBoss2(b, (short)mob.x, (short)mob.y, 72, mob.hp, mob.maxHp, 3);
				}
				if (mob.isBoss)
				{
					newBoss = new NewBoss(b, (short)mob.x, (short)mob.y, mob.templateId, mob.hp, mob.maxHp, mob.sys);
				}
				if (newBoss != null)
				{
					GameScr.vMob.addElement(newBoss);
				}
				else if (bigBoss != null)
				{
					GameScr.vMob.addElement(bigBoss);
				}
				else if (bachTuoc != null)
				{
					GameScr.vMob.addElement(bachTuoc);
				}
				else if (bigBoss2 != null)
				{
					GameScr.vMob.addElement(bigBoss2);
				}
				else
				{
					GameScr.vMob.addElement(mob);
				}
			}
			for (int j = 0; j < Mob.lastMob.size(); j++)
			{
				string text = (string)Mob.lastMob.elementAt(j);
				if (!Mob.isExistNewMob(text))
				{
					Mob.arrMobTemplate[int.Parse(text)].data = null;
					Mob.lastMob.removeElementAt(j);
					j--;
				}
			}
			if (Char.myCharz().mobMe != null && GameScr.findMobInMap(Char.myCharz().mobMe.mobId) == null)
			{
				Char.myCharz().mobMe.getData();
				Char.myCharz().mobMe.x = Char.myCharz().cx;
				Char.myCharz().mobMe.y = Char.myCharz().cy - 40;
				GameScr.vMob.addElement(Char.myCharz().mobMe);
			}
			num = msg.reader().readByte();
			for (byte b2 = 0; b2 < num; b2 = (byte)(b2 + 1))
			{
			}
			GameCanvas.debug("SA75x6", 2);
			num = msg.reader().readByte();
			Res.outz("NPC size= " + num);
			for (int k = 0; k < num; k++)
			{
				sbyte b3 = msg.reader().readByte();
				short cx = msg.reader().readShort();
				short num2 = msg.reader().readShort();
				sbyte b4 = msg.reader().readByte();
				short num3 = msg.reader().readShort();
				if (b4 != 6 && ((Char.myCharz().taskMaint.taskId >= 7 && (Char.myCharz().taskMaint.taskId != 7 || Char.myCharz().taskMaint.index > 1)) || (b4 != 7 && b4 != 8 && b4 != 9)) && (Char.myCharz().taskMaint.taskId >= 6 || b4 != 16))
				{
					if (b4 == 4)
					{
						GameScr.gI().magicTree = new MagicTree(k, b3, cx, num2, b4, num3);
						Service.gI().magicTree(2);
						GameScr.vNpc.addElement(GameScr.gI().magicTree);
					}
					else
					{
						Npc o = new Npc(k, b3, cx, num2 + 3, b4, num3);
						GameScr.vNpc.addElement(o);
					}
				}
			}
			GameCanvas.debug("SA75x7", 2);
			num = msg.reader().readByte();
			Res.outz("item size = " + num);
			for (int l = 0; l < num; l++)
			{
				short itemMapID = msg.reader().readShort();
				short itemTemplateID = msg.reader().readShort();
				int x = msg.reader().readShort();
				int y = msg.reader().readShort();
				int num4 = msg.reader().readInt();
				short r = 0;
				if (num4 == -2)
				{
					r = msg.reader().readShort();
				}
				ItemMap itemMap = new ItemMap(num4, itemMapID, itemTemplateID, x, y, r);
				bool flag = false;
				for (int m = 0; m < GameScr.vItemMap.size(); m++)
				{
					ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(m);
					if (itemMap2.itemMapID == itemMap.itemMapID)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					GameScr.vItemMap.addElement(itemMap);
				}
			}
			if (!GameCanvas.lowGraphic || (GameCanvas.lowGraphic && (TileMap.mapID == 51 || TileMap.mapID == 103)))
			{
				short num5 = msg.reader().readShort();
				TileMap.vCurrItem.removeAllElements();
				if (mGraphics.zoomLevel == 1)
				{
					BgItem.clearHashTable();
				}
				BgItem.vKeysNew.removeAllElements();
				Res.outz("nItem= " + num5);
				for (int n = 0; n < num5; n++)
				{
					short id = msg.reader().readShort();
					short num6 = msg.reader().readShort();
					short num7 = msg.reader().readShort();
					if (TileMap.getBIById(id) == null)
					{
						continue;
					}
					BgItem bIById = TileMap.getBIById(id);
					BgItem bgItem = new BgItem();
					bgItem.id = id;
					bgItem.idImage = bIById.idImage;
					bgItem.dx = bIById.dx;
					bgItem.dy = bIById.dy;
					bgItem.x = num6 * TileMap.size;
					bgItem.y = num7 * TileMap.size;
					bgItem.layer = bIById.layer;
					if (TileMap.isExistMoreOne(bgItem.id))
					{
						bgItem.trans = ((n % 2 != 0) ? 2 : 0);
						if (TileMap.mapID == 45)
						{
							bgItem.trans = 0;
						}
					}
					Image image = null;
					if (!BgItem.imgNew.containsKey(bgItem.idImage + string.Empty))
					{
						if (mGraphics.zoomLevel == 1)
						{
							image = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
							if (image == null)
							{
								image = Image.createRGBImage(new int[1], 1, 1, bl: true);
								Service.gI().getBgTemplate(bgItem.idImage);
							}
							BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
						}
						else
						{
							bool flag2 = false;
							sbyte[] array = Rms.loadRMS(mGraphics.zoomLevel + "bgItem" + bgItem.idImage);
							if (array != null)
							{
								if (BgItem.newSmallVersion != null)
								{
									Res.outz("Small  last= " + array.Length % 127 + "new Version= " + BgItem.newSmallVersion[bgItem.idImage]);
									if (array.Length % 127 != BgItem.newSmallVersion[bgItem.idImage])
									{
										flag2 = true;
									}
								}
								if (!flag2)
								{
									image = Image.createImage(array, 0, array.Length);
									if (image != null)
									{
										BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
									}
									else
									{
										flag2 = true;
									}
								}
							}
							else
							{
								flag2 = true;
							}
							if (flag2)
							{
								image = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
								if (image == null)
								{
									image = Image.createRGBImage(new int[1], 1, 1, bl: true);
									Service.gI().getBgTemplate(bgItem.idImage);
								}
								BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
							}
						}
						BgItem.vKeysLast.addElement(bgItem.idImage + string.Empty);
					}
					if (!BgItem.isExistKeyNews(bgItem.idImage + string.Empty))
					{
						BgItem.vKeysNew.addElement(bgItem.idImage + string.Empty);
					}
					bgItem.changeColor();
					TileMap.vCurrItem.addElement(bgItem);
				}
				for (int num8 = 0; num8 < BgItem.vKeysLast.size(); num8++)
				{
					string text2 = (string)BgItem.vKeysLast.elementAt(num8);
					if (!BgItem.isExistKeyNews(text2))
					{
						BgItem.imgNew.remove(text2);
						if (BgItem.imgNew.containsKey(text2 + "blend" + 1))
						{
							BgItem.imgNew.remove(text2 + "blend" + 1);
						}
						if (BgItem.imgNew.containsKey(text2 + "blend" + 3))
						{
							BgItem.imgNew.remove(text2 + "blend" + 3);
						}
						BgItem.vKeysLast.removeElementAt(num8);
						num8--;
					}
				}
				BackgroudEffect.isFog = false;
				BackgroudEffect.nCloud = 0;
				EffecMn.vEff.removeAllElements();
				BackgroudEffect.vBgEffect.removeAllElements();
				Effect.newEff.removeAllElements();
				short num9 = msg.reader().readShort();
				for (int num10 = 0; num10 < num9; num10++)
				{
					string key = msg.reader().readUTF();
					string value = msg.reader().readUTF();
					keyValueAction(key, value);
				}
				for (int num11 = 0; num11 < Effect.lastEff.size(); num11++)
				{
					string text3 = (string)Effect.lastEff.elementAt(num11);
					if (!Effect.isExistNewEff(text3))
					{
						Effect.removeEffData(int.Parse(text3));
						Effect.lastEff.removeElementAt(num11);
						num11--;
					}
				}
			}
			else
			{
				short num12 = msg.reader().readShort();
				for (int num13 = 0; num13 < num12; num13++)
				{
					short num14 = msg.reader().readShort();
					short num15 = msg.reader().readShort();
					short num16 = msg.reader().readShort();
				}
				short num17 = msg.reader().readShort();
				for (int num18 = 0; num18 < num17; num18++)
				{
					string text4 = msg.reader().readUTF();
					string text5 = msg.reader().readUTF();
				}
			}
			TileMap.bgType = msg.reader().readByte();
			sbyte teleport = msg.reader().readByte();
			loadCurrMap(teleport);
			Char.isLoadingMap = false;
			GameCanvas.debug("SA75x8", 2);
			Resources.UnloadUnusedAssets();
			GC.Collect();
			Cout.LogError("----------DA CHAY XONG LOAD INFO MAP");
		}
		catch (Exception ex)
		{
			Cout.LogError("LOI TAI LOADMAP INFO " + ex.ToString());
		}
	}

	public void keyValueAction(string key, string value)
	{
		if (key.Equals("eff"))
		{
			if (Panel.graphics > 0)
			{
				return;
			}
			string[] array = Res.split(value, ".", 0);
			int id = int.Parse(array[0]);
			int layer = int.Parse(array[1]);
			int x = int.Parse(array[2]);
			int y = int.Parse(array[3]);
			int loop;
			int loopCount;
			if (array.Length <= 4)
			{
				loop = -1;
				loopCount = 1;
			}
			else
			{
				loop = int.Parse(array[4]);
				loopCount = int.Parse(array[5]);
			}
			Effect effect = new Effect(id, x, y, layer, loop, loopCount);
			if (array.Length > 6)
			{
				effect.typeEff = int.Parse(array[6]);
				if (array.Length > 7)
				{
					effect.indexFrom = int.Parse(array[7]);
					effect.indexTo = int.Parse(array[8]);
				}
			}
			EffecMn.addEff(effect);
		}
		else if (key.Equals("beff") && Panel.graphics <= 1)
		{
			BackgroudEffect.addEffect(int.Parse(value));
		}
	}

	public void messageNotMap(Message msg)
	{
		GameCanvas.debug("SA6", 2);
		try
		{
			switch (msg.reader().readByte())
			{
			case 16:
				MoneyCharge.gI().switchToMe();
				break;
			case 17:
				GameCanvas.debug("SYB123", 2);
				Char.myCharz().clearTask();
				break;
			case 18:
			{
				GameCanvas.isLoading = false;
				GameCanvas.endDlg();
				int num2 = msg.reader().readInt();
				GameCanvas.inputDlg.show(mResources.changeNameChar, new Command(mResources.OK, GameCanvas.instance, 88829, num2), TField.INPUT_TYPE_ANY);
				break;
			}
			case 20:
				Char.myCharz().cPk = msg.reader().readByte();
				GameScr.info1.addInfo(mResources.PK_NOW + " " + Char.myCharz().cPk, 0);
				break;
			case 35:
				GameCanvas.endDlg();
				GameScr.gI().resetButton();
				GameScr.info1.addInfo(msg.reader().readUTF(), 0);
				break;
			case 36:
				GameScr.typeActive = msg.reader().readByte();
				Res.outz("load Me Active: " + GameScr.typeActive);
				break;
			case 4:
			{
				GameCanvas.debug("SA8", 2);
				GameCanvas.loginScr.savePass();
				GameScr.isAutoPlay = false;
				GameScr.canAutoPlay = false;
				LoginScr.isUpdateAll = true;
				LoginScr.isUpdateData = true;
				LoginScr.isUpdateMap = true;
				LoginScr.isUpdateSkill = true;
				LoginScr.isUpdateItem = true;
				GameScr.vsData = msg.reader().readByte();
				GameScr.vsMap = msg.reader().readByte();
				GameScr.vsSkill = msg.reader().readByte();
				GameScr.vsItem = msg.reader().readByte();
				sbyte b2 = msg.reader().readByte();
				if (GameCanvas.loginScr.isLogin2)
				{
					Rms.saveRMSString("acc", string.Empty);
					Rms.saveRMSString("pass", string.Empty);
				}
				else
				{
					Rms.saveRMSString("userAo" + ServerListScreen.ipSelect, string.Empty);
				}
				if (GameScr.vsData != GameScr.vcData)
				{
					GameScr.isLoadAllData = false;
					Service.gI().updateData();
				}
				else
				{
					try
					{
						LoginScr.isUpdateData = false;
					}
					catch (Exception)
					{
						GameScr.vcData = -1;
						Service.gI().updateData();
					}
				}
				if (GameScr.vsMap != GameScr.vcMap)
				{
					GameScr.isLoadAllData = false;
					Service.gI().updateMap();
				}
				else
				{
					try
					{
						if (!GameScr.isLoadAllData)
						{
							DataInputStream dataInputStream = new DataInputStream(Rms.loadRMS("NRmap"));
							createMap(dataInputStream.r);
						}
						LoginScr.isUpdateMap = false;
					}
					catch (Exception)
					{
						GameScr.vcMap = -1;
						Service.gI().updateMap();
					}
				}
				if (GameScr.vsSkill != GameScr.vcSkill)
				{
					GameScr.isLoadAllData = false;
					Service.gI().updateSkill();
				}
				else
				{
					try
					{
						if (!GameScr.isLoadAllData)
						{
							DataInputStream dataInputStream2 = new DataInputStream(Rms.loadRMS("NRskill"));
							createSkill(dataInputStream2.r);
						}
						LoginScr.isUpdateSkill = false;
					}
					catch (Exception)
					{
						GameScr.vcSkill = -1;
						Service.gI().updateSkill();
					}
				}
				if (GameScr.vsItem != GameScr.vcItem)
				{
					GameScr.isLoadAllData = false;
					Service.gI().updateItem();
				}
				else
				{
					try
					{
						DataInputStream dataInputStream3 = new DataInputStream(Rms.loadRMS("NRitem0"));
						loadItemNew(dataInputStream3.r, 0, isSave: false);
						DataInputStream dataInputStream4 = new DataInputStream(Rms.loadRMS("NRitem1"));
						loadItemNew(dataInputStream4.r, 1, isSave: false);
						DataInputStream dataInputStream5 = new DataInputStream(Rms.loadRMS("NRitem2"));
						loadItemNew(dataInputStream5.r, 2, isSave: false);
						DataInputStream dataInputStream6 = new DataInputStream(Rms.loadRMS("NRitem100"));
						loadItemNew(dataInputStream6.r, 100, isSave: false);
						LoginScr.isUpdateItem = false;
					}
					catch (Exception)
					{
						GameScr.vcItem = -1;
						Service.gI().updateItem();
					}
				}
				if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
				{
					if (!GameScr.isLoadAllData)
					{
						GameScr.gI().readDart();
						GameScr.gI().readEfect();
						GameScr.gI().readArrow();
						GameScr.gI().readSkill();
					}
					Service.gI().clientOk();
				}
				sbyte b3 = msg.reader().readByte();
				Res.outz("CAPTION LENT= " + b3);
				GameScr.exps = new long[b3];
				for (int j = 0; j < GameScr.exps.Length; j++)
				{
					GameScr.exps[j] = msg.reader().readLong();
				}
				break;
			}
			case 6:
			{
				Res.outz("GET UPDATE_MAP " + msg.reader().available() + " bytes");
				msg.reader().mark(100000);
				createMap(msg.reader());
				msg.reader().reset();
				sbyte[] data3 = new sbyte[msg.reader().available()];
				msg.reader().readFully(ref data3);
				Rms.saveRMS("NRmap", data3);
				sbyte[] data4 = new sbyte[1] { GameScr.vcMap };
				Rms.saveRMS("NRmapVersion", data4);
				LoginScr.isUpdateMap = false;
				if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
				{
					GameScr.gI().readDart();
					GameScr.gI().readEfect();
					GameScr.gI().readArrow();
					GameScr.gI().readSkill();
					Service.gI().clientOk();
				}
				break;
			}
			case 7:
			{
				Res.outz("GET UPDATE_SKILL " + msg.reader().available() + " bytes");
				msg.reader().mark(100000);
				createSkill(msg.reader());
				msg.reader().reset();
				sbyte[] data = new sbyte[msg.reader().available()];
				msg.reader().readFully(ref data);
				Rms.saveRMS("NRskill", data);
				sbyte[] data2 = new sbyte[1] { GameScr.vcSkill };
				Rms.saveRMS("NRskillVersion", data2);
				LoginScr.isUpdateSkill = false;
				if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
				{
					GameScr.gI().readDart();
					GameScr.gI().readEfect();
					GameScr.gI().readArrow();
					GameScr.gI().readSkill();
					Service.gI().clientOk();
				}
				break;
			}
			case 8:
				Res.outz("GET UPDATE_ITEM " + msg.reader().available() + " bytes");
				createItemNew(msg.reader());
				break;
			case 10:
				try
				{
					Char.isLoadingMap = true;
					Res.outz("REQUEST MAP TEMPLATE");
					GameCanvas.isLoading = true;
					TileMap.maps = null;
					TileMap.types = null;
					mSystem.gcc();
					GameCanvas.debug("SA99", 2);
					TileMap.tmw = msg.reader().readByte();
					TileMap.tmh = msg.reader().readByte();
					TileMap.maps = new int[TileMap.tmw * TileMap.tmh];
					Res.outz("   M apsize= " + TileMap.tmw * TileMap.tmh);
					for (int i = 0; i < TileMap.maps.Length; i++)
					{
						int num = msg.reader().readByte();
						if (num < 0)
						{
							num += 256;
						}
						TileMap.maps[i] = (ushort)num;
					}
					TileMap.types = new int[TileMap.maps.Length];
					msg = messWait;
					loadInfoMap(msg);
					try
					{
						sbyte b = msg.reader().readByte();
						TileMap.isMapDouble = ((b != 0) ? true : false);
					}
					catch (Exception)
					{
					}
				}
				catch (Exception ex2)
				{
					Cout.LogError("LOI TAI CASE REQUEST_MAPTEMPLATE " + ex2.ToString());
				}
				msg.cleanup();
				messWait.cleanup();
				msg = (messWait = null);
				break;
			case 12:
				GameCanvas.debug("SA10", 2);
				break;
			case 9:
				GameCanvas.debug("SA11", 2);
				break;
			}
		}
		catch (Exception)
		{
			Cout.LogError("LOI TAI messageNotMap + " + msg.command);
		}
		finally
		{
			msg?.cleanup();
		}
	}

	public void messageNotLogin(Message msg)
	{
		try
		{
			sbyte b = msg.reader().readByte();
			if (b != 2)
			{
				return;
			}
			string linkDefault = msg.reader().readUTF();
			if (mSystem.clientType == 1)
			{
				ServerListScreen.linkDefault = linkDefault;
			}
			else
			{
				ServerListScreen.linkDefault = linkDefault;
			}
			ServerListScreen.getServerList(ServerListScreen.linkDefault);
			try
			{
				sbyte b2 = msg.reader().readByte();
				Panel.CanNapTien = b2 == 1;
			}
			catch (Exception)
			{
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			msg?.cleanup();
		}
	}

	public void messageSubCommand(Message msg)
	{
		try
		{
			GameCanvas.debug("SA12", 2);
			switch (msg.reader().readByte())
			{
			case 63:
			{
				sbyte b = msg.reader().readByte();
				if (b > 0)
				{
					InfoDlg.showWait();
					MyVector vPlayerMenu = GameCanvas.panel.vPlayerMenu;
					for (int n = 0; n < b; n++)
					{
						string caption = msg.reader().readUTF();
						string caption2 = msg.reader().readUTF();
						short menuSelect = msg.reader().readShort();
						Char.myCharz().charFocus.menuSelect = menuSelect;
						Command command = new Command(caption, 11115, Char.myCharz().charFocus);
						command.caption2 = caption2;
						vPlayerMenu.addElement(command);
					}
					InfoDlg.hide();
					GameCanvas.panel.setTabPlayerMenu();
				}
				break;
			}
			case 1:
				GameCanvas.debug("SA13", 2);
				Char.myCharz().nClass = GameScr.nClasss[msg.reader().readByte()];
				Char.myCharz().cTiemNang = msg.reader().readLong();
				Char.myCharz().vSkill.removeAllElements();
				Char.myCharz().vSkillFight.removeAllElements();
				Char.myCharz().myskill = null;
				break;
			case 2:
			{
				GameCanvas.debug("SA14", 2);
				if (Char.myCharz().statusMe != 14 && Char.myCharz().statusMe != 5)
				{
					Char.myCharz().cHP = Char.myCharz().cHPFull;
					Char.myCharz().cMP = Char.myCharz().cMPFull;
					Cout.LogError2(" ME_LOAD_SKILL");
				}
				Char.myCharz().vSkill.removeAllElements();
				Char.myCharz().vSkillFight.removeAllElements();
				sbyte b2 = msg.reader().readByte();
				for (sbyte b3 = 0; b3 < b2; b3 = (sbyte)(b3 + 1))
				{
					short skillId2 = msg.reader().readShort();
					Skill skill5 = Skills.get(skillId2);
					useSkill(skill5);
				}
				GameScr.gI().sortSkill();
				if (GameScr.isPaintInfoMe)
				{
					GameScr.indexRow = -1;
					GameScr.gI().left = (GameScr.gI().center = null);
				}
				break;
			}
			case 19:
				GameCanvas.debug("SA17", 2);
				Char.myCharz().boxSort();
				break;
			case 20:
			{
				GameCanvas.debug("SA18", 2);
				int num3 = msg.reader().readInt();
				Char.myCharz().xu -= num3;
				Char.myCharz().xuInBox += num3;
				break;
			}
			case 21:
			{
				GameCanvas.debug("SA19", 2);
				int num2 = msg.reader().readInt();
				Char.myCharz().xuInBox -= num2;
				Char.myCharz().xu += num2;
				break;
			}
			case 0:
			{
				GameCanvas.debug("SA21", 2);
				RadarScr.list = new MyVector();
				Teleport.vTeleport.removeAllElements();
				GameScr.vCharInMap.removeAllElements();
				GameScr.vItemMap.removeAllElements();
				Char.vItemTime.removeAllElements();
				GameScr.loadImg();
				GameScr.currentCharViewInfo = Char.myCharz();
				Char.myCharz().charID = msg.reader().readInt();
				Char.myCharz().ctaskId = msg.reader().readByte();
				Char.myCharz().cgender = msg.reader().readByte();
				Char.myCharz().head = msg.reader().readShort();
				Char.myCharz().cName = msg.reader().readUTF();
				Char.myCharz().cPk = msg.reader().readByte();
				Char.myCharz().cTypePk = msg.reader().readByte();
				Char.myCharz().cPower = msg.reader().readLong();
				Char.myCharz().applyCharLevelPercent();
				Char.myCharz().eff5BuffHp = msg.reader().readShort();
				Char.myCharz().eff5BuffMp = msg.reader().readShort();
				Char.myCharz().nClass = GameScr.nClasss[msg.reader().readByte()];
				Char.myCharz().vSkill.removeAllElements();
				Char.myCharz().vSkillFight.removeAllElements();
				GameScr.gI().dHP = Char.myCharz().cHP;
				GameScr.gI().dMP = Char.myCharz().cMP;
				sbyte b2 = msg.reader().readByte();
				for (sbyte b4 = 0; b4 < b2; b4 = (sbyte)(b4 + 1))
				{
					Skill skill6 = Skills.get(msg.reader().readShort());
					useSkill(skill6);
				}
				GameScr.gI().sortSkill();
				GameScr.gI().loadSkillShortcut();
				Char.myCharz().xu = msg.reader().readInt();
				Char.myCharz().luongKhoa = msg.reader().readInt();
				Char.myCharz().luong = msg.reader().readInt();
				Char.myCharz().arrItemBody = new Item[msg.reader().readByte()];
				try
				{
					Char.myCharz().setDefaultPart();
					for (int num5 = 0; num5 < Char.myCharz().arrItemBody.Length; num5++)
					{
						short num6 = msg.reader().readShort();
						if (num6 == -1)
						{
							continue;
						}
						ItemTemplate itemTemplate = ItemTemplates.get(num6);
						int num7 = itemTemplate.type;
						Char.myCharz().arrItemBody[num5] = new Item();
						Char.myCharz().arrItemBody[num5].template = itemTemplate;
						Char.myCharz().arrItemBody[num5].quantity = msg.reader().readInt();
						Char.myCharz().arrItemBody[num5].info = msg.reader().readUTF();
						Char.myCharz().arrItemBody[num5].content = msg.reader().readUTF();
						int num8 = msg.reader().readUnsignedByte();
						if (num8 != 0)
						{
							Char.myCharz().arrItemBody[num5].itemOption = new ItemOption[num8];
							for (int num9 = 0; num9 < Char.myCharz().arrItemBody[num5].itemOption.Length; num9++)
							{
								int num10 = msg.reader().readUnsignedByte();
								int param = msg.reader().readUnsignedShort();
								if (num10 != -1)
								{
									Char.myCharz().arrItemBody[num5].itemOption[num9] = new ItemOption(num10, param);
								}
							}
						}
						switch (num7)
						{
						case 0:
							Res.outz("toi day =======================================" + Char.myCharz().body);
							Char.myCharz().body = Char.myCharz().arrItemBody[num5].template.part;
							break;
						case 1:
							Char.myCharz().leg = Char.myCharz().arrItemBody[num5].template.part;
							Res.outz("toi day =======================================" + Char.myCharz().leg);
							break;
						}
					}
				}
				catch (Exception)
				{
				}
				Char.myCharz().arrItemBag = new Item[msg.reader().readByte()];
				GameScr.hpPotion = 0;
				for (int num11 = 0; num11 < Char.myCharz().arrItemBag.Length; num11++)
				{
					short num12 = msg.reader().readShort();
					if (num12 == -1)
					{
						continue;
					}
					Char.myCharz().arrItemBag[num11] = new Item();
					Char.myCharz().arrItemBag[num11].template = ItemTemplates.get(num12);
					Char.myCharz().arrItemBag[num11].quantity = msg.reader().readInt();
					Char.myCharz().arrItemBag[num11].info = msg.reader().readUTF();
					Char.myCharz().arrItemBag[num11].content = msg.reader().readUTF();
					Char.myCharz().arrItemBag[num11].indexUI = num11;
					sbyte b5 = msg.reader().readByte();
					if (b5 != 0)
					{
						Char.myCharz().arrItemBag[num11].itemOption = new ItemOption[b5];
						for (int num13 = 0; num13 < Char.myCharz().arrItemBag[num11].itemOption.Length; num13++)
						{
							int num14 = msg.reader().readUnsignedByte();
							int param2 = msg.reader().readUnsignedShort();
							if (num14 != -1)
							{
								Char.myCharz().arrItemBag[num11].itemOption[num13] = new ItemOption(num14, param2);
								Char.myCharz().arrItemBag[num11].getCompare();
							}
						}
					}
					if (Char.myCharz().arrItemBag[num11].template.type == 6)
					{
						GameScr.hpPotion += Char.myCharz().arrItemBag[num11].quantity;
					}
				}
				Char.myCharz().arrItemBox = new Item[msg.reader().readByte()];
				GameCanvas.panel.hasUse = 0;
				for (int num15 = 0; num15 < Char.myCharz().arrItemBox.Length; num15++)
				{
					short num16 = msg.reader().readShort();
					if (num16 == -1)
					{
						continue;
					}
					Char.myCharz().arrItemBox[num15] = new Item();
					Char.myCharz().arrItemBox[num15].template = ItemTemplates.get(num16);
					Char.myCharz().arrItemBox[num15].quantity = msg.reader().readInt();
					Char.myCharz().arrItemBox[num15].info = msg.reader().readUTF();
					Char.myCharz().arrItemBox[num15].content = msg.reader().readUTF();
					Char.myCharz().arrItemBox[num15].itemOption = new ItemOption[msg.reader().readByte()];
					for (int num17 = 0; num17 < Char.myCharz().arrItemBox[num15].itemOption.Length; num17++)
					{
						int num18 = msg.reader().readUnsignedByte();
						int param3 = msg.reader().readUnsignedShort();
						if (num18 != -1)
						{
							Char.myCharz().arrItemBox[num15].itemOption[num17] = new ItemOption(num18, param3);
							Char.myCharz().arrItemBox[num15].getCompare();
						}
					}
					GameCanvas.panel.hasUse++;
				}
				Char.myCharz().statusMe = 4;
				int num19 = Rms.loadRMSInt(Char.myCharz().cName + "vci");
				if (num19 < 1)
				{
					GameScr.isViewClanInvite = false;
				}
				else
				{
					GameScr.isViewClanInvite = true;
				}
				short num20 = msg.reader().readShort();
				Char.idHead = new short[num20];
				Char.idAvatar = new short[num20];
				for (int num21 = 0; num21 < num20; num21++)
				{
					Char.idHead[num21] = msg.reader().readShort();
					Char.idAvatar[num21] = msg.reader().readShort();
				}
				for (int num22 = 0; num22 < GameScr.info1.charId.Length; num22++)
				{
					GameScr.info1.charId[num22] = new int[3];
				}
				GameScr.info1.charId[Char.myCharz().cgender][0] = msg.reader().readShort();
				GameScr.info1.charId[Char.myCharz().cgender][1] = msg.reader().readShort();
				GameScr.info1.charId[Char.myCharz().cgender][2] = msg.reader().readShort();
				Char.myCharz().isNhapThe = msg.reader().readByte() == 1;
				Res.outz("NHAP THE= " + Char.myCharz().isNhapThe);
				GameScr.deltaTime = mSystem.currentTimeMillis() - (long)msg.reader().readInt() * 1000L;
				GameScr.isNewMember = msg.reader().readByte();
				Service.gI().updateCaption((sbyte)Char.myCharz().cgender);
				Service.gI().androidPack();
				break;
			}
			case 4:
				GameCanvas.debug("SA23", 2);
				Char.myCharz().xu = msg.reader().readInt();
				Char.myCharz().luong = msg.reader().readInt();
				Char.myCharz().cHP = msg.readInt3Byte();
				Char.myCharz().cMP = msg.readInt3Byte();
				Char.myCharz().luongKhoa = msg.reader().readInt();
				break;
			case 5:
			{
				GameCanvas.debug("SA24", 2);
				int cHP = Char.myCharz().cHP;
				Char.myCharz().cHP = msg.readInt3Byte();
				if (Char.myCharz().cHP > cHP && Char.myCharz().cTypePk != 4)
				{
					GameScr.startFlyText("+" + (Char.myCharz().cHP - cHP) + " " + mResources.HP, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 20, 0, -1, mFont.HP);
					SoundMn.gI().HP_MPup();
					if (Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5003)
					{
						MonsterDart.addMonsterDart(Char.myCharz().petFollow.cmx + ((Char.myCharz().petFollow.dir != 1) ? (-10) : 10), Char.myCharz().petFollow.cmy + 10, isBoss: true, -1, -1, Char.myCharz(), 29);
					}
				}
				if (Char.myCharz().cHP < cHP)
				{
					GameScr.startFlyText("-" + (cHP - Char.myCharz().cHP) + " " + mResources.HP, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 20, 0, -1, mFont.HP);
				}
				GameScr.gI().dHP = Char.myCharz().cHP;
				if (GameScr.isPaintInfoMe)
				{
				}
				break;
			}
			case 6:
			{
				GameCanvas.debug("SA25", 2);
				if (Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5)
				{
					break;
				}
				int cMP = Char.myCharz().cMP;
				Char.myCharz().cMP = msg.readInt3Byte();
				if (Char.myCharz().cMP > cMP)
				{
					GameScr.startFlyText("+" + (Char.myCharz().cMP - cMP) + " " + mResources.KI, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 23, 0, -2, mFont.MP);
					SoundMn.gI().HP_MPup();
					if (Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5001)
					{
						MonsterDart.addMonsterDart(Char.myCharz().petFollow.cmx + ((Char.myCharz().petFollow.dir != 1) ? (-10) : 10), Char.myCharz().petFollow.cmy + 10, isBoss: true, -1, -1, Char.myCharz(), 29);
					}
				}
				if (Char.myCharz().cMP < cMP)
				{
					GameScr.startFlyText("-" + (cMP - Char.myCharz().cMP) + " " + mResources.KI, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 23, 0, -2, mFont.MP);
				}
				Res.outz("curr MP= " + Char.myCharz().cMP);
				GameScr.gI().dMP = Char.myCharz().cMP;
				if (GameScr.isPaintInfoMe)
				{
				}
				break;
			}
			case 7:
			{
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char != null)
				{
					@char.clanID = msg.reader().readInt();
					if (@char.clanID == -2)
					{
						@char.isCopy = true;
					}
					readCharInfo(@char, msg);
				}
				break;
			}
			case 8:
			{
				GameCanvas.debug("SA26", 2);
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char != null)
				{
					@char.cspeed = msg.reader().readByte();
				}
				break;
			}
			case 9:
			{
				GameCanvas.debug("SA27", 2);
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char != null)
				{
					@char.cHP = msg.readInt3Byte();
					@char.cHPFull = msg.readInt3Byte();
				}
				break;
			}
			case 10:
			{
				GameCanvas.debug("SA28", 2);
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char != null)
				{
					@char.cHP = msg.readInt3Byte();
					@char.cHPFull = msg.readInt3Byte();
					@char.eff5BuffHp = msg.reader().readShort();
					@char.eff5BuffMp = msg.reader().readShort();
					@char.wp = msg.reader().readShort();
					if (@char.wp == -1)
					{
						@char.setDefaultWeapon();
					}
				}
				break;
			}
			case 11:
			{
				GameCanvas.debug("SA29", 2);
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char != null)
				{
					@char.cHP = msg.readInt3Byte();
					@char.cHPFull = msg.readInt3Byte();
					@char.eff5BuffHp = msg.reader().readShort();
					@char.eff5BuffMp = msg.reader().readShort();
					@char.body = msg.reader().readShort();
					if (@char.body == -1)
					{
						@char.setDefaultBody();
					}
				}
				break;
			}
			case 12:
			{
				GameCanvas.debug("SA30", 2);
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char != null)
				{
					@char.cHP = msg.readInt3Byte();
					@char.cHPFull = msg.readInt3Byte();
					@char.eff5BuffHp = msg.reader().readShort();
					@char.eff5BuffMp = msg.reader().readShort();
					@char.leg = msg.reader().readShort();
					if (@char.leg == -1)
					{
						@char.setDefaultLeg();
					}
				}
				break;
			}
			case 13:
			{
				GameCanvas.debug("SA31", 2);
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char != null)
				{
					@char.cHP = msg.readInt3Byte();
					@char.cHPFull = msg.readInt3Byte();
					@char.eff5BuffHp = msg.reader().readShort();
					@char.eff5BuffMp = msg.reader().readShort();
				}
				break;
			}
			case 14:
			{
				GameCanvas.debug("SA32", 2);
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char == null)
				{
					break;
				}
				@char.cHP = msg.readInt3Byte();
				sbyte b6 = msg.reader().readByte();
				Res.outz("player load hp type= " + b6);
				if (b6 == 1)
				{
					ServerEffect.addServerEffect(11, @char, 5);
					ServerEffect.addServerEffect(104, @char, 4);
				}
				try
				{
					@char.cHPFull = msg.readInt3Byte();
					break;
				}
				catch (Exception)
				{
					break;
				}
			}
			case 15:
			{
				GameCanvas.debug("SA33", 2);
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char != null)
				{
					@char.cHP = msg.readInt3Byte();
					@char.cHPFull = msg.readInt3Byte();
					@char.cx = msg.reader().readShort();
					@char.cy = msg.reader().readShort();
					@char.statusMe = 1;
					@char.cp3 = 3;
					ServerEffect.addServerEffect(109, @char, 2);
				}
				break;
			}
			case 35:
			{
				GameCanvas.debug("SY3", 2);
				int num = msg.reader().readInt();
				Res.outz("CID = " + num);
				if (TileMap.mapID == 130)
				{
					GameScr.gI().starVS();
				}
				if (num == Char.myCharz().charID)
				{
					Char.myCharz().cTypePk = msg.reader().readByte();
					if (GameScr.gI().isVS() && Char.myCharz().cTypePk != 0)
					{
						GameScr.gI().starVS();
					}
					Res.outz("type pk= " + Char.myCharz().cTypePk);
					Char.myCharz().npcFocus = null;
					if (!GameScr.gI().isMeCanAttackMob(Char.myCharz().mobFocus))
					{
						Char.myCharz().mobFocus = null;
					}
					Char.myCharz().itemFocus = null;
				}
				else
				{
					Char @char = GameScr.findCharInMap(num);
					if (@char != null)
					{
						Res.outz("type pk= " + @char.cTypePk);
						@char.cTypePk = msg.reader().readByte();
						if (@char.isAttacPlayerStatus())
						{
							Char.myCharz().charFocus = @char;
						}
					}
				}
				for (int m = 0; m < GameScr.vCharInMap.size(); m++)
				{
					Char char2 = GameScr.findCharInMap(m);
					if (char2 != null && char2.cTypePk != 0 && char2.cTypePk == Char.myCharz().cTypePk)
					{
						if (!Char.myCharz().mobFocus.isMobMe)
						{
							Char.myCharz().mobFocus = null;
						}
						Char.myCharz().npcFocus = null;
						Char.myCharz().itemFocus = null;
						break;
					}
				}
				Res.outz("update type pk= ");
				break;
			}
			case 61:
			{
				string text = msg.reader().readUTF();
				sbyte[] data = new sbyte[msg.reader().readInt()];
				msg.reader().read(ref data);
				if (data.Length == 0)
				{
					data = null;
				}
				if (text.Equals("KSkill"))
				{
					GameScr.gI().onKSkill(data);
				}
				else if (text.Equals("OSkill"))
				{
					GameScr.gI().onOSkill(data);
				}
				else if (text.Equals("CSkill"))
				{
					GameScr.gI().onCSkill(data);
				}
				break;
			}
			case 23:
			{
				short num4 = msg.reader().readShort();
				Skill skill4 = Skills.get(num4);
				useSkill(skill4);
				if (num4 != 0 && num4 != 14 && num4 != 28)
				{
					GameScr.info1.addInfo(mResources.LEARN_SKILL + " " + skill4.template.name, 0);
				}
				break;
			}
			case 62:
			{
				Res.outz("ME UPDATE SKILL");
				short skillId = msg.reader().readShort();
				Skill skill = Skills.get(skillId);
				for (int i = 0; i < Char.myCharz().vSkill.size(); i++)
				{
					Skill skill2 = (Skill)Char.myCharz().vSkill.elementAt(i);
					if (skill2.template.id == skill.template.id)
					{
						Char.myCharz().vSkill.setElementAt(skill, i);
						break;
					}
				}
				for (int j = 0; j < Char.myCharz().vSkillFight.size(); j++)
				{
					Skill skill3 = (Skill)Char.myCharz().vSkillFight.elementAt(j);
					if (skill3.template.id == skill.template.id)
					{
						Char.myCharz().vSkillFight.setElementAt(skill, j);
						break;
					}
				}
				for (int k = 0; k < GameScr.onScreenSkill.Length; k++)
				{
					if (GameScr.onScreenSkill[k] != null && GameScr.onScreenSkill[k].template.id == skill.template.id)
					{
						GameScr.onScreenSkill[k] = skill;
						break;
					}
				}
				for (int l = 0; l < GameScr.keySkill.Length; l++)
				{
					if (GameScr.keySkill[l] != null && GameScr.keySkill[l].template.id == skill.template.id)
					{
						GameScr.keySkill[l] = skill;
						break;
					}
				}
				if (Char.myCharz().myskill.template.id == skill.template.id)
				{
					Char.myCharz().myskill = skill;
				}
				GameScr.info1.addInfo(mResources.hasJustUpgrade1 + skill.template.name + mResources.hasJustUpgrade2 + skill.point, 0);
				break;
			}
			}
		}
		catch (Exception ex3)
		{
			Cout.println("Loi tai Sub : " + ex3.ToString());
		}
		finally
		{
			msg?.cleanup();
		}
	}

	private void useSkill(Skill skill)
	{
		if (Char.myCharz().myskill == null)
		{
			Char.myCharz().myskill = skill;
		}
		else if (skill.template.Equals(Char.myCharz().myskill.template))
		{
			Char.myCharz().myskill = skill;
		}
		Char.myCharz().vSkill.addElement(skill);
		if ((skill.template.type == 1 || skill.template.type == 4 || skill.template.type == 2 || skill.template.type == 3) && (skill.template.maxPoint == 0 || (skill.template.maxPoint > 0 && skill.point > 0)))
		{
			if (skill.template.id == Char.myCharz().skillTemplateId)
			{
				Service.gI().selectSkill(Char.myCharz().skillTemplateId);
			}
			Char.myCharz().vSkillFight.addElement(skill);
		}
	}

	public bool readCharInfo(Char c, Message msg)
	{
		try
		{
			c.clevel = msg.reader().readByte();
			c.isInvisiblez = msg.reader().readBoolean();
			c.cTypePk = msg.reader().readByte();
			Res.outz("ADD TYPE PK= " + c.cTypePk + " to player " + c.charID + " @@ " + c.cName);
			c.nClass = GameScr.nClasss[msg.reader().readByte()];
			c.cgender = msg.reader().readByte();
			c.head = msg.reader().readShort();
			c.cName = msg.reader().readUTF();
			c.cHP = msg.readInt3Byte();
			c.dHP = c.cHP;
			if (c.cHP == 0)
			{
				c.statusMe = 14;
			}
			c.cHPFull = msg.readInt3Byte();
			if (c.cy >= TileMap.pxh - 100)
			{
				c.isFlyUp = true;
			}
			c.body = msg.reader().readShort();
			c.leg = msg.reader().readShort();
			c.bag = msg.reader().readUnsignedByte();
			Res.outz(" body= " + c.body + " leg= " + c.leg + " bag=" + c.bag + "BAG ==" + c.bag + "*********************************");
			c.isShadown = true;
			sbyte b = msg.reader().readByte();
			if (c.wp == -1)
			{
				c.setDefaultWeapon();
			}
			if (c.body == -1)
			{
				c.setDefaultBody();
			}
			if (c.leg == -1)
			{
				c.setDefaultLeg();
			}
			c.cx = msg.reader().readShort();
			c.cy = msg.reader().readShort();
			c.xSd = c.cx;
			c.ySd = c.cy;
			c.eff5BuffHp = msg.reader().readShort();
			c.eff5BuffMp = msg.reader().readShort();
			int num = msg.reader().readByte();
			for (int i = 0; i < num; i++)
			{
				EffectChar effectChar = new EffectChar(msg.reader().readByte(), msg.reader().readInt(), msg.reader().readInt(), msg.reader().readShort());
				c.vEff.addElement(effectChar);
				if (effectChar.template.type == 12 || effectChar.template.type == 11)
				{
					c.isInvisiblez = true;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			ex.StackTrace.ToString();
		}
		return false;
	}

	private void readGetImgByName(Message msg)
	{
		try
		{
			string text = msg.reader().readUTF();
			sbyte nFrame = msg.reader().readByte();
			sbyte[] array = null;
			array = NinjaUtil.readByteArray(msg);
			Image img = createImage(array);
			ImgByName.SetImage(text, img, nFrame);
			if (array != null)
			{
				ImgByName.saveRMS(text, nFrame, array);
			}
		}
		catch (Exception)
		{
		}
	}

	private void createItemNew(myReader d)
	{
		try
		{
			loadItemNew(d, -1, isSave: true);
		}
		catch (Exception)
		{
		}
	}

	private void loadItemNew(myReader d, sbyte type, bool isSave)
	{
		try
		{
			d.mark(100000);
			GameScr.vcItem = d.readByte();
			type = d.readByte();
			if (type == 0)
			{
				GameScr.gI().iOptionTemplates = new ItemOptionTemplate[d.readUnsignedByte()];
				for (int i = 0; i < GameScr.gI().iOptionTemplates.Length; i++)
				{
					GameScr.gI().iOptionTemplates[i] = new ItemOptionTemplate();
					GameScr.gI().iOptionTemplates[i].id = i;
					GameScr.gI().iOptionTemplates[i].name = d.readUTF();
					GameScr.gI().iOptionTemplates[i].type = d.readByte();
				}
				if (isSave)
				{
					d.reset();
					sbyte[] data = new sbyte[d.available()];
					d.readFully(ref data);
					Rms.saveRMS("NRitem0", data);
				}
			}
			else if (type == 1)
			{
				ItemTemplates.itemTemplates.clear();
				int num = d.readShort();
				for (int j = 0; j < num; j++)
				{
					ItemTemplate it = new ItemTemplate((short)j, d.readByte(), d.readByte(), d.readUTF(), d.readUTF(), d.readByte(), d.readInt(), d.readShort(), d.readShort(), d.readBoolean());
					ItemTemplates.add(it);
				}
				if (isSave)
				{
					d.reset();
					sbyte[] data2 = new sbyte[d.available()];
					d.readFully(ref data2);
					Rms.saveRMS("NRitem1", data2);
				}
			}
			else if (type == 2)
			{
				int num2 = d.readShort();
				int num3 = d.readShort();
				for (int k = num2; k < num3; k++)
				{
					ItemTemplate it2 = new ItemTemplate((short)k, d.readByte(), d.readByte(), d.readUTF(), d.readUTF(), d.readByte(), d.readInt(), d.readShort(), d.readShort(), d.readBoolean());
					ItemTemplates.add(it2);
				}
				if (isSave)
				{
					d.reset();
					sbyte[] data3 = new sbyte[d.available()];
					d.readFully(ref data3);
					Rms.saveRMS("NRitem2", data3);
					sbyte[] data4 = new sbyte[1] { GameScr.vcItem };
					Rms.saveRMS("NRitemVersion", data4);
					LoginScr.isUpdateItem = false;
					if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
					{
						GameScr.gI().readDart();
						GameScr.gI().readEfect();
						GameScr.gI().readArrow();
						GameScr.gI().readSkill();
						Service.gI().clientOk();
					}
				}
			}
			else if (type == 100)
			{
				Char.Arr_Head_2Fr = readArrHead(d);
				if (isSave)
				{
					d.reset();
					sbyte[] data5 = new sbyte[d.available()];
					d.readFully(ref data5);
					Rms.saveRMS("NRitem100", data5);
				}
			}
		}
		catch (Exception ex)
		{
			ex.ToString();
		}
	}

	private void readFrameBoss(Message msg, int mobTemplateId)
	{
		try
		{
			int num = msg.reader().readByte();
			int[][] array = new int[num][];
			for (int i = 0; i < num; i++)
			{
				int num2 = msg.reader().readByte();
				array[i] = new int[num2];
				for (int j = 0; j < num2; j++)
				{
					array[i][j] = msg.reader().readByte();
				}
			}
			frameHT_NEWBOSS.put(mobTemplateId + string.Empty, array);
		}
		catch (Exception)
		{
		}
	}

	private int[][] readArrHead(myReader d)
	{
		int[][] array = new int[1][] { new int[2] { 542, 543 } };
		try
		{
			int num = d.readShort();
			array = new int[num][];
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = d.readByte();
				array[i] = new int[num2];
				for (int j = 0; j < num2; j++)
				{
					array[i][j] = d.readShort();
				}
			}
			return array;
		}
		catch (Exception)
		{
			return array;
		}
	}
}
