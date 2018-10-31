using System;
using CarHunters.Core.ViewModels.Abstract;
using MvvmCross.Commands;
using CarHunters.Core.Common.Abstractions;
using CarHunters.Core.Resources;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;
using CarHunters.Core.Common.Models.Messages;
using CarHunters.Core.Common.Models;
using CarHunters.Core.Units.ML.Services.Abstractions;
using System.Drawing;
using CarHunters.Core.Common.Extensions;
using MvvmCross.ViewModels;
using System.Collections.Generic;

namespace CarHunters.Core.ViewModels
{
    public class HuntViewModel : BaseViewModel
    {
        private readonly IPermissionsService permissionsService;
        private readonly INotificationHubService notificationHubService;
        private readonly IEntityAccessorService entityAccessorService;

        public HuntViewModel(IPermissionsService permissionsService,
                             INotificationHubService notificationHubService,
                             IEntityAccessorService entityAccessorService)
        {
            this.permissionsService = permissionsService;
            this.notificationHubService = notificationHubService;
            this.entityAccessorService = entityAccessorService;
            this.notificationHubService.OnNewFrame += NewFrameIncoming;
            this.entityAccessorService.LogChanged += ScreenLogChanged;
            this.entityAccessorService.ObjectTracker.BboxChanged += ObjectTrackerBboxChanged;
            this.entityAccessorService.ObjectTracker.LogChanged += ScreenLogChanged;
            this.entityAccessorService.VehicleDetector.VehicleFound += VehicleDetectorVehicleFound;
            this.entityAccessorService.CarBBoxDetector.LogChanged += ScreenLogChanged;
            this.entityAccessorService.TopFeaturesDetector.LogChanged += ScreenLogChanged;
            Subscriptions.Add(Messenger.Subscribe<ChangeStateAppMessage>(msg => AppStateChanged(msg)));

            CreateItemsSource();
        }

        void CreateItemsSource()
        {
            ItemsSource = new MvxObservableCollection<MappedLogParams>
            {
                new MappedLogParams(),
                new MappedLogParams(),
                new MappedLogParams(),
                new MappedLogParams(),
                new MappedLogParams(),
                new MappedLogParams(),
                new MappedLogParams(),
                new MappedLogParams(),
                new MappedLogParams(),
                new MappedLogParams()
            };
        }

        private MvxObservableCollection<MappedLogParams> _itemsSource;
        public MvxObservableCollection<MappedLogParams> ItemsSource
        {
            get => _itemsSource;
            set => SetProperty(ref _itemsSource, value);
        }

        private RectangleF? _boundingBox;
        public RectangleF? BoundingBox
        {
            get => _boundingBox;
            set => SetProperty(ref _boundingBox, value);
        }

        private RectangleF? _detectedCarBbox;
        public RectangleF? DetectedCarBbox
        {
            get => _detectedCarBbox;
            set => SetProperty(ref _detectedCarBbox, value);
        }

        private CustomColor _boundingBoxColor;
        public CustomColor BoundingBoxColor
        {
            get => _boundingBoxColor;
            set => SetProperty(ref _boundingBoxColor, value);
        }

        private CustomColor _detectedCarBoxColor;
        public CustomColor DetectedCarBoxColor
        {
            get => _detectedCarBoxColor;
            set => SetProperty(ref _detectedCarBoxColor, value);
        }

        void ObjectTrackerBboxChanged(object sender, BboxParams trackedCarParams)
        {
            BoundingBox = trackedCarParams.Bbox;
            BoundingBoxColor = trackedCarParams.Color.ToOurColor();
        }

        void VehicleDetectorVehicleFound(object sender, BboxParams vehicleParams)
        {
            DetectedCarBbox = vehicleParams.Bbox;
            DetectedCarBoxColor = vehicleParams.Color.ToOurColor();
        }

        void ScreenLogChanged(object sender, LogParams logParams)
        {
            ItemsSource[logParams.ShowIdx] = new MappedLogParams(logParams);
        }

        void NewFrameIncoming(object sender, FrameEntry frameEntry)
        {
            if (frameEntry == null)
                return;

            entityAccessorService.VehicleDetector.LastFrame = frameEntry;
            entityAccessorService.ObjectTracker.NextFrame(frameEntry);
        }

        //TODO: NEED TO THINK REALY WE NEED TO CHECK CAMERA ON INITIALIZE
        //public override async Task Initialize()
        //{
        //    await CheckCamera();
        //}

        async void AppStateChanged(ChangeStateAppMessage message)
        {
            if(!message.InBackground)
            {
                await CheckCamera();
            }
        }

        private async Task CheckCamera()
        {
            var precheckResult = await permissionsService.PreCheckPermissionsAccessAsync(Permission.Camera);
            {
                switch(precheckResult)
                {
                    case PermissionStatus.Unknown:
                        CameraAccessGranted = await permissionsService.CheckPermissionsAccesGrantedAsync(Permission.Camera);
                        break;
                    case PermissionStatus.Granted:
                        CameraAccessGranted = true;
                        break;
                    default:
                        CameraAccessGranted = false;
                        break;
                }
            }
        }

        public override async void OnResume()
        {
            base.OnResume();
            await CheckCamera();
            entityAccessorService.ResumeEntities();
        }

        private bool cameraAccessGranted;
        public bool CameraAccessGranted
        {
            get => cameraAccessGranted;
            set => SetProperty(ref cameraAccessGranted, value);
        }

        private MvxAsyncCommand _openSettingsCommand;
        public IMvxAsyncCommand OpenSettingsCommand
        {
            get
            {
                return _openSettingsCommand ?? (_openSettingsCommand = new MvxAsyncCommand(async () => 
                {
                    if(!permissionsService.Instance.OpenAppSettings())
                    {
                        await UserInteractionService.AlertAsync(Translator.GetText("CannotOpenSettingsString"));
                        return;
                    }
                }));
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            entityAccessorService.StopEntities();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            entityAccessorService.StopEntities();
            notificationHubService.OnNewFrame -= NewFrameIncoming;
            entityAccessorService.ObjectTracker.BboxChanged -= ObjectTrackerBboxChanged;
            entityAccessorService.ObjectTracker.LogChanged -= ScreenLogChanged;
            entityAccessorService.VehicleDetector.VehicleFound -= VehicleDetectorVehicleFound;
            entityAccessorService.CarBBoxDetector.LogChanged -= ScreenLogChanged;
        }
    }
}
