function net=bann2(x,y,xx,yy,trainf,hm,f1,f2,tn,traino)

%有监控集的神经网络训练函数
%net:随机训练50个周期得到最佳网络参数；
%x:校正集光谱矩阵；
%y:校正集性质向量；
%xx:监控集光谱矩阵；
%yy：监控集性质矩阵；
%trainf:训练函数，取值为traingd，traingdm，，trainbfg和trainlm中的一个
%hm：隐含节点数；
%f1:第一层传递函数，取值为tansig,logsig,purelin中的一个；
%f2:第二层传递函数，取值为tansig,logsig,purelin中的一个；
%tn:训练次数；
%traino:训练目标.

aa=inf;
nn=[];
v.P=xx;
v.T=yy;
for i=1:50
    net=newff(minmax(x),[hm,1],{f1,f2},trainf);
    net.trainParam.lr=0.0002;
    net.trainParam.epochs = tn;
    net.trainParam.goal = traino;
    net=train(net,x,y,[],[],v);
    a1=sim(net,x);
    seca=(sumsqr(y-a1)/(length(a1)-1)).^0.5;
    a2=sim(net,xx);
    sepa=(sumsqr(yy-a2)/(length(a2)-1)).^0.5;
    summ=seca+sepa;
    if summ<aa
        aa=summ;
        nn=net;
    end
end 
 
net=nn;