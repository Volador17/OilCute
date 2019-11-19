function [y,SEP,vR,ye,md,rms,nnd]=plsannv(x,vy,net,Weights,Loads,b,my,centerSpecData,Score_length);
    
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
 mmy=mean(vy);   
[m,n]=size(x);
x=x';
calx = x - ones(size(x,1),1) * centerSpecData;
calx=calx';
[Scores] =plssc2(calx,Loads,Weights,b,Score_length);;%计算验证集得分。
md=diag(Scores*Scores');;
%计算光谱残差
X =calx-Loads*Scores';  
rms=(sum((X).^2)).^0.5;
nnd=nndr(Scores',Scores');
y=sim(net,Scores')+my*ones(1,n);%计算预测性质；

ye=y-vy;%计算残差；
vR=(sum((y-mmy).*(y-mmy)))/(sum(ye.*ye)+sum((y-mmy).*(y-mmy)));%计算相关系数；
SEP=sqrt(ye*ye'/(n-1));%计算预测标准偏差
