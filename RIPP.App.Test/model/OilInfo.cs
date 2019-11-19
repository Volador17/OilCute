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
    public partial class OilInfo: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public string crudeName
        {
            get { return _crudeName; }
            set
            {
                if (_crudeName != value)
                {
                    _crudeName = value;
                    OnPropertyChanged("crudeName");
                }
            }
        }
        private string _crudeName;
    
        [DataMember]
        public string englishName
        {
            get { return _englishName; }
            set
            {
                if (_englishName != value)
                {
                    _englishName = value;
                    OnPropertyChanged("englishName");
                }
            }
        }
        private string _englishName;
    
        [DataMember]
        public string crudeIndex
        {
            get { return _crudeIndex; }
            set
            {
                if (_crudeIndex != value)
                {
                    _crudeIndex = value;
                    OnPropertyChanged("crudeIndex");
                }
            }
        }
        private string _crudeIndex;
    
        [DataMember]
        public string country
        {
            get { return _country; }
            set
            {
                if (_country != value)
                {
                    _country = value;
                    OnPropertyChanged("country");
                }
            }
        }
        private string _country;
    
        [DataMember]
        public string region
        {
            get { return _region; }
            set
            {
                if (_region != value)
                {
                    _region = value;
                    OnPropertyChanged("region");
                }
            }
        }
        private string _region;
    
        [DataMember]
        public string fieldBlock
        {
            get { return _fieldBlock; }
            set
            {
                if (_fieldBlock != value)
                {
                    _fieldBlock = value;
                    OnPropertyChanged("fieldBlock");
                }
            }
        }
        private string _fieldBlock;
    
        [DataMember]
        public Nullable<System.DateTime> sampleDate
        {
            get { return _sampleDate; }
            set
            {
                if (_sampleDate != value)
                {
                    _sampleDate = value;
                    OnPropertyChanged("sampleDate");
                }
            }
        }
        private Nullable<System.DateTime> _sampleDate;
    
        [DataMember]
        public Nullable<System.DateTime> receiveDate
        {
            get { return _receiveDate; }
            set
            {
                if (_receiveDate != value)
                {
                    _receiveDate = value;
                    OnPropertyChanged("receiveDate");
                }
            }
        }
        private Nullable<System.DateTime> _receiveDate;
    
        [DataMember]
        public string sampleSite
        {
            get { return _sampleSite; }
            set
            {
                if (_sampleSite != value)
                {
                    _sampleSite = value;
                    OnPropertyChanged("sampleSite");
                }
            }
        }
        private string _sampleSite;
    
        [DataMember]
        public Nullable<System.DateTime> assayDate
        {
            get { return _assayDate; }
            set
            {
                if (_assayDate != value)
                {
                    _assayDate = value;
                    OnPropertyChanged("assayDate");
                }
            }
        }
        private Nullable<System.DateTime> _assayDate;
    
        [DataMember]
        public Nullable<System.DateTime> updataDate
        {
            get { return _updataDate; }
            set
            {
                if (_updataDate != value)
                {
                    _updataDate = value;
                    OnPropertyChanged("updataDate");
                }
            }
        }
        private Nullable<System.DateTime> _updataDate;
    
        [DataMember]
        public string sourceRef
        {
            get { return _sourceRef; }
            set
            {
                if (_sourceRef != value)
                {
                    _sourceRef = value;
                    OnPropertyChanged("sourceRef");
                }
            }
        }
        private string _sourceRef;
    
        [DataMember]
        public string assayLab
        {
            get { return _assayLab; }
            set
            {
                if (_assayLab != value)
                {
                    _assayLab = value;
                    OnPropertyChanged("assayLab");
                }
            }
        }
        private string _assayLab;
    
        [DataMember]
        public string assayer
        {
            get { return _assayer; }
            set
            {
                if (_assayer != value)
                {
                    _assayer = value;
                    OnPropertyChanged("assayer");
                }
            }
        }
        private string _assayer;
    
        [DataMember]
        public string assayCustomer
        {
            get { return _assayCustomer; }
            set
            {
                if (_assayCustomer != value)
                {
                    _assayCustomer = value;
                    OnPropertyChanged("assayCustomer");
                }
            }
        }
        private string _assayCustomer;
    
        [DataMember]
        public string reportIndex
        {
            get { return _reportIndex; }
            set
            {
                if (_reportIndex != value)
                {
                    _reportIndex = value;
                    OnPropertyChanged("reportIndex");
                }
            }
        }
        private string _reportIndex;
    
        [DataMember]
        public string summary
        {
            get { return _summary; }
            set
            {
                if (_summary != value)
                {
                    _summary = value;
                    OnPropertyChanged("summary");
                }
            }
        }
        private string _summary;
    
        [DataMember]
        public string type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("type");
                }
            }
        }
        private string _type;
    
        [DataMember]
        public string classification
        {
            get { return _classification; }
            set
            {
                if (_classification != value)
                {
                    _classification = value;
                    OnPropertyChanged("classification");
                }
            }
        }
        private string _classification;
    
        [DataMember]
        public string sulfurLevel
        {
            get { return _sulfurLevel; }
            set
            {
                if (_sulfurLevel != value)
                {
                    _sulfurLevel = value;
                    OnPropertyChanged("sulfurLevel");
                }
            }
        }
        private string _sulfurLevel;
    
        [DataMember]
        public string acidLevel
        {
            get { return _acidLevel; }
            set
            {
                if (_acidLevel != value)
                {
                    _acidLevel = value;
                    OnPropertyChanged("acidLevel");
                }
            }
        }
        private string _acidLevel;
    
        [DataMember]
        public string corrosionLevel
        {
            get { return _corrosionLevel; }
            set
            {
                if (_corrosionLevel != value)
                {
                    _corrosionLevel = value;
                    OnPropertyChanged("corrosionLevel");
                }
            }
        }
        private string _corrosionLevel;
    
        [DataMember]
        public string processingIndex
        {
            get { return _processingIndex; }
            set
            {
                if (_processingIndex != value)
                {
                    _processingIndex = value;
                    OnPropertyChanged("processingIndex");
                }
            }
        }
        private string _processingIndex;
    
        [DataMember]
        public Nullable<bool> isLibraryA
        {
            get { return _isLibraryA; }
            set
            {
                if (_isLibraryA != value)
                {
                    _isLibraryA = value;
                    OnPropertyChanged("isLibraryA");
                }
            }
        }
        private Nullable<bool> _isLibraryA;
    
        [DataMember]
        public Nullable<bool> isLibraryB
        {
            get { return _isLibraryB; }
            set
            {
                if (_isLibraryB != value)
                {
                    _isLibraryB = value;
                    OnPropertyChanged("isLibraryB");
                }
            }
        }
        private Nullable<bool> _isLibraryB;

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
