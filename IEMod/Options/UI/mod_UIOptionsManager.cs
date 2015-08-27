using System;
using System.IO;
using System.Linq;
using IEMod.Helpers;
using IEMod.QuickControls;
using IEMod.QuickControls.Controls;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Options {



	[ModifiesType]
	public class mod_UIOptionsManager : UIOptionsManager
	{
		[ReplaceType("UIOptionsManager/OptionsPage")]
		public new enum OptionsPage
		{
			MENU = 1,
			GAME = 2,
			AUTOPAUSE = 4,
			DIFFICULTY = 8,
			GRAPHICS = 16,
			SOUND = 32,
			CONTROLS = 64,
			CUSTOM = 128,
			CUSTOM2 = 256
		}
		[ModifiesType("MappedInput")]
		public class Mod4_MappedInput {
			[ModifiesAccessibility]
			public static bool m_ControlNames;
			[ModifiesAccessibility]
			public static bool ReadOnlyControls;
		}
		[NewMember]
		private QuickCheckbox _blueCircles;
		[NewMember]
		private QuickCheckbox _blueCirclesBg;
		[NewMember]
		private QuickCheckbox _alwaysShowCircles;
		[NewMember]
		private QuickCheckbox _unlockCombatInv;
		[NewMember]
		private QuickCheckbox _fixBackerNames;
		[NewMember]
		private QuickCheckbox _removeMovingRecovery;
		[NewMember]
		private QuickCheckbox _fastSneak;
		[NewMember]
		private QuickCheckbox _improvedAi;
		[NewMember]
		private QuickCheckbox _disableFfCb;
		[NewMember]
		private QuickCheckbox _oneTooltip;
		[NewMember]
		private QuickCheckbox _disableEngagement;
		[NewMember]
		private QuickDropdown<IEModOptions.NerfedXpTable> _nerfedXpCmb;
		[NewMember]
		private QuickCheckbox _lootShuffler;
		[NewMember]
		private QuickCheckbox _gameSpeed;
		[NewMember]
		private QuickCheckbox _combatOnly;
		[NewMember]
		private QuickCheckbox _bonusSpellsPerDay;
		[NewMember]
		private QuickCheckbox _targetTurnedEnemies;
		[NewMember]
		private QuickCheckbox _npcDispositionFix;
		[NewMember]
		private QuickDropdown<IEModOptions.PerEncounterSpells> _perEncounterSpellsCmb;
		[NewMember]
		private QuickDropdown<IEModOptions.ExtraSpellsInGrimoire> _extraGrimoireSpellsCmb;
		[NewMember]
		private QuickDropdown<IEModOptions.AutoSaveSetting> _autosaveCmb;

		[NewMember]
		private QuickCheckbox _disableBackerDialog;
		
		[NewMember]
		[DuplicatesBody("Start")]
		public void StartOrig() {
			throw new DeadEndException("StartOrig");
		}

		[ModifiesMember("Start")]
		private void StartNew() {
			var exampleCheckbox =
				this.ComponentsInDescendants<UIOptionsTag>(true).Single(
					opt => opt.Checkbox && opt.BoolSuboption == GameOption.BoolOption.SCREEN_EDGE_SCROLLING)
					.transform;

			Prefabs.QuickCheckbox = exampleCheckbox.Component<UIOptionsTag>();
			Prefabs.QuickDropdown = ResolutionDropdown.transform.parent.gameObject;
			Prefabs.QuickButton = UIOptionsManager.Instance.PageButtonPrefab.gameObject;
			Prefabs.QuickPage = Pages[5];

			var pageParent = Pages[5].transform.parent;

			var ieModOptions = new QuickPage(pageParent, "IEModOptions_Settings_Page");
			var ieModDisposition = new QuickPage(pageParent, "IEModOptions_Disposition_Page");
			Pages = Pages.Concat(new[] {
				ieModOptions.GameObject,
				ieModDisposition.GameObject
			}).ToArray();

			//don't touch this
			this.SetMenuLayout(OptionsMenuLayout.InGame);
			this.QuitButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.QuitButton.onClick, new UIEventListener.VoidDelegate(this.OnQuitClicked));
			this.SaveButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.SaveButton.onClick, new UIEventListener.VoidDelegate(this.OnSaveClicked));
			this.LoadButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.LoadButton.onClick, new UIEventListener.VoidDelegate(this.OnLoadClicked));
        
			this.m_Options = this.ComponentsInDescendants<UIOptionsTag>(true);

			var quickFactory = new QuickFactory() {
				CurrentParent = Pages[7].transform
			};

			// end
			//File.WriteAllText("ComboboxDump.txt", UnityObjectDumper.PrintUnityGameObject(exampleDropdown.gameObject, null, x => false));
			quickFactory.CurrentParent = Pages[7].transform;
			//The following are the controls that appear in the GUI of the mod.
			_oneTooltip = quickFactory.Checkbox(() => IEModOptions.OneTooltip);
			_oneTooltip.Transform.localPosition = new Vector3(-210, 330, 0);

			_disableEngagement = quickFactory.Checkbox(() => IEModOptions.DisableEngagement);
			_disableEngagement.LocalPosition = new Vector3(-210, 300, 0);

			_blueCircles = quickFactory.Checkbox(() => IEModOptions.BlueCircles);
			_blueCircles.Transform.localPosition = new Vector3(-210, 270, 0);

			_blueCirclesBg = quickFactory.Checkbox(() => IEModOptions.BlueCirclesBG);
			_blueCirclesBg.Transform.localPosition = new Vector3(-180, 240, 0);

			_blueCircles.IsChecked.HasChanged += x => {
				if (x.Value) {
					_blueCirclesBg.OptionsTagComponent.Enable();
				} else {
					_blueCirclesBg.IsChecked.Value = false;
					_blueCirclesBg.OptionsTagComponent.Disable();
				}
			};
			_blueCircles.IsChecked.NotifyChange();

			_alwaysShowCircles = quickFactory.Checkbox(() => IEModOptions.AlwaysShowCircles);
			_alwaysShowCircles.Transform.localPosition = new Vector3(-210, 210, 0);

			_unlockCombatInv = quickFactory.Checkbox(() => IEModOptions.UnlockCombatInv);
			_unlockCombatInv.Transform.localPosition = new Vector3(-210, 180, 0);

			_disableBackerDialog = quickFactory.Checkbox(() => IEModOptions.DisableBackerDialogs);
			_disableBackerDialog.Transform.localPosition = new Vector3(230, 120, 0);

			_fixBackerNames = quickFactory.Checkbox(() => IEModOptions.FixBackerNames);
			_fixBackerNames.IsChecked.OnChange(v => {
				if (v.Value) {
					_disableBackerDialog.OptionsTagComponent.Enable();
				} else {
					_disableBackerDialog.IsChecked.Value = false;
					_disableBackerDialog.OptionsTagComponent.Disable();
				}
			});
			_fixBackerNames.Transform.localPosition = new Vector3(210, 150, 0);



			_npcDispositionFix = quickFactory.Checkbox(() => IEModOptions.NPCDispositionFix);
			_npcDispositionFix.Transform.localPosition = new Vector3(-210, 150, 0);

			_removeMovingRecovery = quickFactory.Checkbox(() => IEModOptions.RemoveMovingRecovery);
			_removeMovingRecovery.Transform.localPosition = new Vector3(-210, 120, 0);

			_fastSneak = quickFactory.Checkbox(() => IEModOptions.FastSneak);
			_fastSneak.Transform.localPosition = new Vector3(-210, 90, 0);

			_improvedAi = quickFactory.Checkbox(() => IEModOptions.ImprovedAI);
			_improvedAi.Transform.localPosition = new Vector3(-210, 60, 0);

			_disableFfCb = quickFactory.Checkbox(() => IEModOptions.DisableFriendlyFire);
			_disableFfCb.Transform.localPosition = new Vector3(-210, 30, 0);

			_lootShuffler = quickFactory.Checkbox(() => IEModOptions.LootShuffler);
			_lootShuffler.Transform.localPosition = new Vector3(210, 300, 0);

			_gameSpeed = quickFactory.Checkbox(() => IEModOptions.GameSpeedMod);
			_gameSpeed.Transform.localPosition = new Vector3(210, 270, 0);

			_combatOnly = quickFactory.Checkbox(() => IEModOptions.CombatOnlyMod);
			_combatOnly.Transform.localPosition = new Vector3(210, 240, 0);

			_bonusSpellsPerDay = quickFactory.Checkbox(() => IEModOptions.BonusSpellsPerDay);
			_bonusSpellsPerDay.Transform.localPosition = new Vector3(210, 210, 0);

			_targetTurnedEnemies = quickFactory.Checkbox(() => IEModOptions.TargetTurnedEnemies);
			_targetTurnedEnemies.Transform.localPosition = new Vector3(210, 180, 0);
		
			
			_nerfedXpCmb = quickFactory.EnumDropdown(() => IEModOptions.NerfedXPTableSetting);
			_nerfedXpCmb.Width = 515;
			_nerfedXpCmb.LabelWidth = 300;
			_nerfedXpCmb.LocalPosition = new Vector3(-80, -70, 0);

			_perEncounterSpellsCmb = quickFactory.EnumDropdown(() => IEModOptions.PerEncounterSpellsSetting);
			_perEncounterSpellsCmb.LabelWidth = 300;
			_perEncounterSpellsCmb.Width = 515;
			_perEncounterSpellsCmb.LocalPosition = new Vector3(-80, -110, 0);

			_extraGrimoireSpellsCmb = quickFactory.EnumDropdown(() => IEModOptions.ExtraWizardSpells);
			_extraGrimoireSpellsCmb.Width = 515;
			_extraGrimoireSpellsCmb.LabelWidth = 300;
			_extraGrimoireSpellsCmb.Transform.localPosition = new Vector3(-80, -150, 0);

			_autosaveCmb = quickFactory.EnumDropdown(() => IEModOptions.AutosaveSetting);
			_autosaveCmb.Width = 515;
			_autosaveCmb.LabelWidth = 300;
			_autosaveCmb.Transform.localPosition = new Vector3(-80, -30, 0);

			// add autosave settings
			// end of adding checkbox

			// Pallegina dispositions mod page
			quickFactory.CurrentParent = ieModDisposition.Transform;

			var favoredDisposition1 = quickFactory.EnumDropdown(() => IEModOptions.PalleginaFavored1);
			favoredDisposition1.Width = 150;
			favoredDisposition1.LabelWidth = 300;
			favoredDisposition1.LocalPosition = new Vector3(-60, 300, 0);

			var favoredDisposition2 = quickFactory.EnumDropdown(() => IEModOptions.PalleginaFavored2);
			favoredDisposition2.LabelWidth = 0;
			favoredDisposition2.Width = 150;
			favoredDisposition2.LocalPosition = new Vector3(100, 300, 0);

			var disDisposition1 = quickFactory.EnumDropdown(() => IEModOptions.PalleginaDisfavored1);
			disDisposition1.Width = 150;
			disDisposition1.LabelWidth = 300;
			disDisposition1.LocalPosition = new Vector3(-60, 250, 0);

			var disDisposition2 = quickFactory.EnumDropdown(() => IEModOptions.PalleginaDisfavored2);
			disDisposition2.LocalPosition = new Vector3(100, 250, 0);
			disDisposition2.LabelWidth = 0;
			disDisposition2.Width = 150;
			
			//END OF CONTROL DEFINITIONS. The rest is built in stuff that needs to happen after creating all of our controls.
			this.PageButtonGroup.OnRadioSelectionChangedEvent += new UIRadioButtonGroup.RadioSelectionChanged(this.OnChangePage); // changed to Event
			this.m_PageButtonGrid = this.PageButtonGroup.GetComponent<UIGrid>();
			this.m_PageButtons = new UIMultiSpriteImageButton[9];
			this.m_PageButtons[0] = this.PageButtonPrefab;
			for (int i = 0; i < 9; i++)
			{
				if (i > 0)
				{
					this.m_PageButtons[i] = NGUITools.AddChild(this.PageButtonPrefab.transform.parent.gameObject, this.PageButtonPrefab.gameObject).GetComponent<UIMultiSpriteImageButton>();
				}
				if (i < this.PageTitleStringIds.Length)
				{
					GUIStringLabel.Get(this.m_PageButtons[i].Label).SetString(this.PageTitleStringIds[i]);
				}
				else if(i == this.PageTitleStringIds.Length) // added this line
				{
					GUIStringLabel.Get (this.m_PageButtons [i].Label).FormatString = "IE Mod"; // added this line
					//Debug.LogWarning("Not enough strings provided for every options tab in OptionsManager.");
				}
				else if (i == this.PageTitleStringIds.Length + 1)
				{
					GUIStringLabel.Get(this.m_PageButtons[i].Label).FormatString = "Dispositions"; // added this line
					//Debug.LogWarning("Not enough strings provided for every options tab in OptionsManager.");
				}
				this.m_PageButtons[i].name = this.PageOrder[i] + "." + this.m_PageButtons[i].name;
			}
			this.m_PageButtonGrid.Reposition();
			foreach (UIOptionsTag tag in this.m_Options)
			{
				if (tag.Checkbox != null)
				{
					tag.Checkbox.onStateChange = (UICheckbox.OnStateChange) Delegate.Combine(tag.Checkbox.onStateChange, new UICheckbox.OnStateChange(this.OnCheckChanged));
				}
			}

			this.CombatTimerSlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.CombatTimerSlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnCombatTimerChanged));
			this.AutoslowThresholdSlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.AutoslowThresholdSlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnAutoslowThresholdChanged));
			this.TooltipDelay.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.TooltipDelay.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnTooltipDelayChanged));
			this.FontSize.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.FontSize.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnFontSizeChanged));
			this.GammaSlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.GammaSlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnGammaSliderChanged));
			this.AreaLootRangeSlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.AreaLootRangeSlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnAreaLootSliderChanged));
			this.VoiceFrequency.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.VoiceFrequency.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnVoiceFrequencyChanged));
			this.ScrollSpeed.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.ScrollSpeed.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnScrollSpeedChanged));

			this.LanguageDropdown.OnDropdownOptionChangedEvent += new UIDropdownMenu.DropdownOptionChanged(this.OnLanguageChanged); // changed to Event
			this.ResolutionDropdown.OnDropdownOptionChangedEvent += new UIDropdownMenu.DropdownOptionChanged(this.OnResolutionChanged); // changed to Event
			this.QualitySlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.QualitySlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnQualityChanged));
			this.FrameRateMaxSlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.FrameRateMaxSlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnMaxFPSChanged));

			this.m_VolumeSliders = new UIOptionsSliderGroup[4];
			UIOptionsVolumeSlider[] componentsInChildren = base.GetComponentsInChildren<UIOptionsVolumeSlider>(true);
			UIOptionsVolumeSlider[] array = componentsInChildren;
			for (int k = 0; k < array.Length; k++)
			{
				UIOptionsVolumeSlider uIOptionsVolumeSlider = array[k];
				if (this.m_VolumeSliders[(int)uIOptionsVolumeSlider.Category] == null)
				{
					UIOptionsSliderGroup component = uIOptionsVolumeSlider.GetComponent<UIOptionsSliderGroup>();
					this.m_VolumeSliders[(int)uIOptionsVolumeSlider.Category] = component;
					component.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged)Delegate.Combine(component.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnVolumeChanged));
				}
			}
			this.AcceptButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.AcceptButton.onClick, new UIEventListener.VoidDelegate(this.OnAcceptClick));
			this.DefControlsButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.DefControlsButton.onClick, new UIEventListener.VoidDelegate(this.OnRestoreDefaultControls));
			this.ApplyResolutionButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.ApplyResolutionButton.onClick, new UIEventListener.VoidDelegate(this.OnApplyResolution));
		}

		[MemberAlias(".ctor", typeof(MonoBehaviour))]
		private void MonoBehavior_ctor() {
			
		}

		[ModifiesMember(".ctor")]
		public void CtorNew() {
			MonoBehavior_ctor();
			PageOrder = new[] {
				0,
				3,
				1,
				4,
				5,
				2,
				6,
				7,
				8
			};
			m_ValidOptionPages = (UIOptionsManager.OptionsPage)OptionsPage.CONTROLS | (UIOptionsManager.OptionsPage)OptionsPage.SOUND | (UIOptionsManager.OptionsPage)OptionsPage.GRAPHICS | (UIOptionsManager.OptionsPage)OptionsPage.GAME | (UIOptionsManager.OptionsPage)OptionsPage.AUTOPAUSE | (UIOptionsManager.OptionsPage)OptionsPage.MENU | (UIOptionsManager.OptionsPage)OptionsPage.CUSTOM | (UIOptionsManager.OptionsPage)OptionsPage.CUSTOM2;
			m_GameMode = new GameMode();
			m_Controls = new ControlMapping();
		}



	}

}