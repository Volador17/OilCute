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
    public partial class OilTableCol: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public int oilTableTypeID
        {
            get { return _oilTableTypeID; }
            set
            {
                if (_oilTableTypeID != value)
                {
                    _oilTableTypeID = value;
                    OnPropertyChanged("oilTableTypeID");
                }
            }
        }
        private int _oilTableTypeID;
    
        [DataMember]
        public string colName
        {
            get { return _colName; }
            set
            {
                if (_colName != value)
                {
                    _colName = value;
                    OnPropertyChanged("colName");
                }
            }
        }
        private string _colName;
    
        [DataMember]
        public int colOrder
        {
            get { return _colOrder; }
            set
            {
                if (_colOrder != value)
                {
                    _colOrder = value;
                    OnPropertyChanged("colOrder");
                }
            }
        }
        private int _colOrder;
    
        [DataMember]
        public Nullable<bool> isDisplay
        {
            get { return _isDisplay; }
            set
            {
                if (_isDisplay != value)
                {
                    _isDisplay = value;
                    OnPropertyChanged("isDisplay");
                }
            }
        }
        private Nullable<bool> _isDisplay;
    
        [DataMember]
        public string descript
        {
            get { return _descript; }
            set
            {
                if (_descript != value)
                {
                    _descript = value;
                    OnPropertyChanged("descript");
                }
            }
        }
        private string _descript;
    
        [DataMember]
        public Nullable<bool> isSystem
        {
            get { return _isSystem; }
            set
            {
                if (_isSystem != value)
                {
                    _isSystem = value;
                    OnPropertyChanged("isSystem");
                }
            }
        }
        private Nullable<bool> _isSystem;

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
