function [y,md,rms,nnd, ss]=plsannp(x,net,Weights,Loads,Scores,b,my,centerSpecData,Score_length);
    
%偏最小二乘人工神经网络预测程序；
%x：光谱矩阵；
%net:网络参数
%Loads:载荷；
%Weights:权重矢量；
%b:回归系数；
%my：y的平均值；
%y：预测性质；
%md:马氏距离；
%rms:光谱残差；
%Score_length   得分向量的长度  
% 校正集光谱中心化值
[m,n]=size(x);
x=x';
valx = x - ones(size(x,1),1) * centerSpecData;
valx=valx';
[ss] =plssc2(valx,Loads,Weights,b,Score_length);;%计算验证集得分。
md=diag(ss*ss');;
%计算光谱残差
X =valx-Loads*ss';  
rms=(sum((X).^2)).^0.5;
nnd=nndr(ss',Scores');
y=sim(net,ss');
y = y +my*ones(1,n);%计算预测性质；
