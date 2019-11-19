function [Ylast,SR,MD,nd,XScores] = PLSPredictorForANN(X,Scores, Loads, Weights, b,  Score_length, centerSpecData, centerCompValue)
% 目标：计算监控集得分
% 函数输入参数： 
%            X              光谱矩阵                        [波长点数 * 未知的波谱个数(样本数)]
%            Scores         校正集谱图得分矩阵               [样品数*主成分数]
%            Loads          校正集谱图载荷矩阵               [波长数*主成分数]
%            Weights        校正集谱图权重矩阵               [波长数*主成分数]
%            Score_length   得分向量的长度                   [1 * 主成分数]
% 函数输出参数:	
%            XScores   未知样本的得分矩阵       [ 未知波普个数 * 主成分个数 ]
% 依据文献: ASTM 标准E 1655-05 第11页 PLS 
X = X -  centerSpecData'* ones(1, size(X,2)) ;

ColsX = size (X,2);
ColsLoads = size(Loads,2); 
Ylast = zeros (ColsX,ColsLoads);
% 初始化分析结果矩阵 其中矩阵的行为k 矩阵的列为n
XScores =zeros (ColsX,ColsLoads);
Factor = ColsLoads;
tx = X;
% 下面是采用 ASTM 上第11页注释进行，能够顺利完成对 未知样本的预测 。经过修改后通过 Scores 进行估计
for i = 1 : Factor
    Weights(:,i)=Weights(:,i)/Score_length(i);
    XScores(:,i)= X'*Weights(:,i);
    XScores(:,i)=XScores(:,i)/Score_length(i);
    MD(:,i)=diag(XScores*XScores');
    tx = tx -  Loads(:,i)*XScores(:,i)';
    SR(i,:)=(sum((tx).^2)).^0.5;
    nd(:,i)=nndr(XScores(:,1:i)',Scores(:,1:i)');
end
SR=SR';
%计算光谱残差

