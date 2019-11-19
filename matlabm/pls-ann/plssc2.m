function [XScores ]=plssc2(X,Loads,Weights,b,Score_length);
% 目标：计算监控集得分
% 函数输入参数： 
%            X              光谱矩阵                        [波长点数 * 未知的波谱个数(样本数)]
%            Scores         校正集谱图得分矩阵               [样品数*主成分数]
%            Loads          校正集谱图载荷矩阵               [波长数*主成分数]
%            Weights        校正集谱图权重矩阵               [波长数*主成分数]
%            b              校正集线性回归函数               [1 * 主成分数]
%            Score_length   得分向量的长度                   [1 * 主成分数]
% 函数输出参数:	
%            XScores   未知样本的得分矩阵       [ 未知波普个数 * 主成分个数 ]
% 依据文献: ASTM 标准E 1655-05 第11页 PLS 
[RowsX,ColsX] = size (X);
[RowsLoads,ColsLoads]=size(Loads); 
% 初始化分析结果矩阵 其中矩阵的行为k 矩阵的列为n
XScores =zeros (ColsX,ColsLoads);
Factor=ColsLoads;
% 下面是采用 ASTM 上第11页注释进行，能够顺利完成对 未知样本的预测 。经过修改后通过 Scores 进行估计
for i = 1 : Factor
    Weights(:,i)=Weights(:,i)/Score_length(i);
    XScores(:,i)= X'*Weights(:,i);
    XScores(:,i)=XScores(:,i)/Score_length(i);
end




