//------------------------------------------------------------------------------
// <auto-generated>
//     此代码是根据模板生成的。
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，则所做更改将丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace RIPP.App.Test.model
{
    [DataContract(IsReference = true)]
    public partial class OilData: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region 基元属性
    
        [DataMember]
        public int ID
        {
            get { return _iD; }
            set
            {
                if (_iD != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("属性“ID”是对象键的一部分，不可更改。仅当未跟踪对象或对象处于“已添加”状态时，才能对键属性进行更改。");
                    }
                    _iD = value;
                    OnPropertyChanged("ID");
                }
            }
        }
        private int _iD;
    
        [DataMember]
        public int oilInfoID
        {
            get { return _oilInfoID; }
            set
            {
                if (_oilInfoID != value)
                {
                    ChangeTracker.RecordOriginalValue("oilInfoID", _oilInfoID);
                    _oilInfoID = value;
                    OnPropertyChanged("oilInfoID");
                }
            }
        }
        private int _oilInfoID;
    
        [DataMember]
        public int oilTableColID
        {
            get { return _oilTableColID; }
            set
            {
                if (_oilTableColID != value)
                {
                    _oilTableColID = value;
                    OnPropertyChanged("oilTableColID");
                }
            }
        }
        private int _oilTableColID;
    
        [DataMember]
        public int oilTableRowID
        {
            get { return _oilTableRowID; }
            set
            {
                if (_oilTableRowID != value)
                {
                    _oilTableRowID = value;
                    OnPropertyChanged("oilTableRowID");
                }
            }
        }
        private int _oilTableRowID;
    
        [DataMember]
        public string labData
        {
            get { return _labData; }
            set
            {
                if (_labData != value)
                {
                    _labData = value;
                    OnPropertyChanged("labData");
                }
            }
        }
        private string _labData;
    
        [DataMember]
        public string calData
        {
            get { return _calData; }
            set
            {
                if (_calData != value)
                {
                    _calData = value;
                    OnPropertyChanged("calData");
                }
            }
        }
        private string _calData;

        #endregion
        #region ChangeTracking
    
        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
            {
                ChangeTracker.State = ObjectState.Modified;
            }
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        protected virtual void OnNavigationPropertyChanged(String propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged{ add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        private event PropertyChangedEventHandler _propertyChanged;
        private ObjectChangeTracker _changeTracker;
    
        [DataMember]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ObjectChangeTracker();
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
                return _changeTracker;
            }
            set
            {
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging -= HandleObjectStateChanging;
                }
                _changeTracker = value;
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
            }
        }
    
        private void HandleObjectStateChanging(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                ClearNavigationProperties();
            }
        }
    
        protected bool IsDeserializing { get; private set; }
    
        [OnDeserializing]
        public void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
        }
    
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            ChangeTracker.ChangeTrackingEnabled = true;
        }
    
        protected virtual void ClearNavigationProperties()
        {
        }

        #endregion
    }
}
