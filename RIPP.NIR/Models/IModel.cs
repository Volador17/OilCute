using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
using RIPPMatlab;

namespace RIPP.NIR.Models
{

    public interface IModel<T> : IDisposable
    {
       /// <summary>
       /// 预测
       /// </summary>
       /// <param name="spec"></param>
        /// <param name="needFilter">是否需要前处理</param>
       /// <returns></returns>
        T Predict(Spectrum spec, bool needFilter = true, int numOfId = 5, int topK = 1);

        /// <summary>
        /// 外部验证
        /// </summary>
        /// <param name="lib"></param>
        /// <param name="needFilter"></param>
        /// <returns></returns>
        T[] Validation(SpecBase lib, bool needFilter = true, int numOfId = 5);


        T[] CrossValidation(SpecBase lib, bool needFilter = true, int numOfId = 5);

        /// <summary>
        /// 训练
        /// </summary>
        /// <param name="lib">光谱库</param>
        /// <param name="needFilter">是否需要前处理</param>
        void Train(SpecBase lib, bool needFilter = true);

     

        //override IEquatable

        /// <summary>
        /// 文件的路径 
        /// </summary>
        /// <param name="fullPath"></param>
        string FullPath { set; get; }

        /// <summary>
        /// 创建者
        /// </summary>
        string Creater { set; get; }



        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { set; get; }

        string Name { set; get; }

        /// <summary>
        /// 设置建模所需要的光谱库
        /// </summary>
        /// <param name="specBasePath"></param>
        SpecBase LibBase { set; get; }

        /// <summary>
        /// 前处理方法集合
        /// </summary>
        IList<Data.Filter.IFilter> Filters { set; get; }

        bool Trained { set; get; }

        /// <summary>
        /// 是否被编辑过
        /// </summary>
        bool Edited { set; get; }

        /// <summary>
        /// 模型保存
        /// </summary>
        /// <returns></returns>
        bool Save();


        /// <summary>
        /// 模型的性质
        /// </summary>
        /// <returns></returns>
        ComponentList GetComponents();

    }
}
