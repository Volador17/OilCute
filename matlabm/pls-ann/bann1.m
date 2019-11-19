function net=bann1(x,y,trainf,hm,f1,f2,tn,traino)

%无监控集的神经网络训练函数
%net:随机训练50个周期得到最佳网络参数；
%x:光谱矩阵；
%y:性质向量；
%trainf:训练函数，取值为traingd，traingdm，，trainbfg和trainlm中的一个
%hm：隐含节点数；
%f1:第一层传递函数，取值为tansig,logsig,purelin中的一个；
%f2:第二层传递函数，取值为tansig,logsig,purelin中的一个；
%tn:训练次数；
%traino:训练目标.


aa=inf;
nn=[];
for i=1:50
    net=newff(minmax(x),[hm,1],{f1,f2},trainf); 
    net.trainParam.lr=0.0002;
    net.trainParam.epochs = tn;
    net.trainParam.goal = traino; 
    net=train(net,x,y); 
    a1=sim(net,x);
    seca=(sumsqr(y-a1)/(length(a1)-1)).^0.5;
    if seca <aa
        aa=seca;
        nn=net;
    end
end 
 
net=nn;