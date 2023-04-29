using Airplane;
using GameState;
using Interactions;
using Interactions.Resources;
using LoadingScreenView;
using System.Collections.Generic;
using UnityEngine;

namespace SceneLoad
{
    public class InteractionObjectsData : MonoBehaviour
    {
        [Header("Objects to init")]
        [SerializeField] private Borders _borders;
        [SerializeField] private LevelProgressionHandler _levelProgressionHandler;
        [Header("Objects to be inited")]
        [SerializeField] private List<GrowBuildingDistrict> _buildingDistricts;

        public Borders Borders => _borders;
        public LevelProgressionHandler LevelProgressionHandler => _levelProgressionHandler;
        public List<GrowBuildingDistrict> BuildingDistricts=> _buildingDistricts;

        public void InitDistricts(PlayerMovement playerMovement, UpgradeResource upgradeResource, DistrictCompleteView districtCompleteView, 
            ViewPanel lowResourceView, ViewPanel needToBuildDirectionView, ViewPanel airplaneUpgradeUI)
        {
            _buildingDistricts.ForEach(disctirct => disctirct.Init(playerMovement, upgradeResource, districtCompleteView, lowResourceView, needToBuildDirectionView, airplaneUpgradeUI)); 
        }
    }
}
