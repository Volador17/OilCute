function [net,Loads,Scores,Weights,b,yreg,ye,R,sec,centerSpecData,centerCompValue,Score_length,mdt,nndt] = PLSANNTrain(X,Y,Factor,trainf,hm,f1,f2,tn,traino,kk,vx,vy)
 
    nanidx = isnan(Y);
    Y(nanidx) = [];
    X(:,nanidx) = [];
    
    if nargin > 10
        nanidx = isnan(vy);
        vy(nanidx) = [];
        vx(:,nanidx) = [];
    end
    
    
    [Scores,Loads, Weights,b,Score_length, centerSpecData , centerCompValue , mdt,nndt ] = PLS1Train(X,Y,Factor);
    if nargin > 10
        valx = vx -  centerSpecData'* ones(1, size(vx,2));
        valy = vy - centerCompValue;
        
        [XScores] =plssc2(valx,Loads,Weights,b,Score_length);%计算监控集得分。
    end
    %-------------------------------------------------------------------------
    
    calY = (Y - centerCompValue)';
    save test.mat Scores calY trainf hm f1 f2 tn traino; 
    for i=1:kk
        if nargin > 10
            net = bann2(Scores',calY,XScores',valy,trainf,hm,f1,f2,tn,traino);%计算网络参数
        else
            net = bann1(Scores',calY,trainf,hm,f1,f2,tn,traino);%计算网络参数
        end
        yy(i,:) = sim(net,Scores');
        error(i,:) = yy(i,:)- (Y-centerCompValue)';
        yy1 = sim(net,Scores') + centerCompValue;%计算预测值；
        yy2(i,:) = yy1;
    end
    yreg = mean(yy2); 
    yreg = yreg';
    ye = yreg - Y; %计算残差；

    R = (sum((yreg-centerCompValue).*(yreg-centerCompValue)))/(sum(ye.*ye)+sum((yreg-centerCompValue).*(yreg-centerCompValue)));%计算相关系数；

    sec = sqrt(ye*ye'/(size(X,2)-1));%计算预测标准偏差

end