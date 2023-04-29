using Airplane;
using BarUI;
using GameState;
using Interactions;
using Interactions.Resources;
using LoadingScreenView;
using System.Collections.Generic;
using UnityEngine;

namespace SceneLoad
{
    public class GameSceneInit : MonoBehaviour
    {
        [Header("Objects to be inited")]
        [SerializeField] private MoverFullControl _moverFullControl;
        [SerializeField] private EndScreenView _endScreenView;
        [SerializeField] private List<BarView> _districtProgressViews;
        [SerializeField] private NeedToBuildDirectionView _needToBuildDirectionView;
        [Header("Objects to init")]
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private UpgradeResource _upgradeResource;
        [SerializeField] private DistrictCompleteView _districtCompleteView;
        [SerializeField] private ViewPanel _lowResourceView;
        [SerializeField] private ViewPanel _airplaneUpgradeUI;

        public void InitScene(InteractionObjectsData interactionObjectsData)
        {
            _moverFullControl.Init(interactionObjectsData.Borders);
            _endScreenView.Init(interactionObjectsData.LevelProgressionHandler);
            _needToBuildDirectionView.Init(interactionObjectsData.LevelProgressionHandler);

            for (int districtIndex = 0; districtIndex < _districtProgressViews.Count; districtIndex += 1)
            {
                _districtProgressViews[districtIndex].Init(interactionObjectsData.BuildingDistricts[districtIndex]);
            }

            interactionObjectsData.InitDistricts(_playerMovement, _upgradeResource, _districtCompleteView, _lowResourceView, _needToBuildDirectionView, _airplaneUpgradeUI);
            interactionObjectsData.LevelProgressionHandler.Init(_playerMovement);
        }
    }
}
