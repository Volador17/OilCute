function [net,Loads,Scores,Weights,b,yreg,ye,md,mdt,my,R,sec,rms,centerSpecData,Score_length,nnd,nndt]=plsann1(x,y,pp,trainf,hm,f1,f2,tn,traino,kk)

%无监控集的偏最小二乘－人工神经网络建模程序；
%net：神经网络参数；
%Loads:载荷；
%Scores:得分；
%Weights:权重矢量；
%b:回归系数；
%yreg:预测值；
%ye：残差；
%md:马氏距离；
%mdt：马氏距离阈值；
%my：y的平均值；
%R:预测值与实际值之间的相关系数。
%sec：校正集预测标准偏差；
%rms：光谱残差。

%x：光谱矩阵；
%y：性质向量；
%pp：主因子数；
%trainf:训练函数，取值为traingd，traingdm，trainbfg和trainlm中的一个
%hm：隐含节点数；
%f1:第一层传递函数，取值为tansig,logsig,purelin中的一个；
%f2:第二层传递函数，取值为tansig,logsig,purelin中的一个；
%tn:训练次数；
%traino:训练目标.
% kk: 外部循环次数

y= y';

[m,n]=size(x);
x=x';
centerSpecData = mean(x);
calx = x - ones(size(x,1),1) * centerSpecData;
calx=calx';
my=mean(y);
caly=y-my;%y的均值中心化；
[Scores,Loads,Weights,b,Score_length] = pls1(calx,caly',pp);%偏最小二乘，得到载荷和得分。
%计算马氏距离以及马氏距离阈值
md=diag(Scores*Scores');
mdt=3*pp/m;
%计算光谱残差
X =calx-Loads*Scores';  
rms=(sum((X).^2)).^0.5;
%计算最邻近距离阈值
nnd=nndr(Scores',Scores');
nndt=max(nnd);
%-------------------------------------------------------------------------
for i=1:kk

net=bann1(Scores',caly,trainf,hm,f1,f2,tn,traino);%计算网络参数

yy1=sim(net,Scores')+my*ones(1,n);%计算预测值；
yy2(i,:)=yy1;
end
yreg=mean(yy2);
ye=yreg-y;%计算残差；

R=(sum((yreg-my).*(yreg-my)))/(sum(ye.*ye)+sum((yreg-my).*(yreg-my)));%计算相关系数；

sec=sqrt(ye*ye'/(n-1));%计算预测标准偏差



