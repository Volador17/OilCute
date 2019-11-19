loadCalData; % 加载cx cy

% PLS1
Factor = 10;
[Scores, Loads, Weights,b,Score_length, centerSpecData , centerCompValue ] = PLS1Train(cx,cy,Factor);

% ANN 参数
hm = 5;             % 隐含节点数
f1 = 'logsig';      % 第一层传递函数，取值为tansig,logsig,purelin中的一个
f2 = 'purelin';     % 第二层传递函数，取值为tansig,logsig,purelin中的一个
trainf = 'trainlm'; % 训练函数，取值为traingd，traingdm，，trainbfg和trainlm中的一个
tn = 100;           % 训练次数
traino = 1.000000000000000e-04;% 训练目标

calY = (cy - centerCompValue)';
Scores = Scores';

minMSE = inf;
bestNet =[];
for i=1:50
    net = newff(minmax(Scores),[hm,1],{f1,f2},trainf); 
    net.trainParam.lr = 0.0002;
    net.trainParam.epochs = tn;
    net.trainParam.goal = traino; 
    net=train(net,Scores,calY); 
    preY(i,:) = sim(net,Scores);
    mseData(i) = mse(calY - preY(i,:) );
    if mseData(i)< minMSE
        minMSE = mseData(i);
        bestNet = net;
    end
    %se(i) = sumsqr(calY-preY(i,:))/length(preY(i,:));
end 

% 将 Scores和calY 写入文件，供FANN使用
[row, col] = size(Scores);
fid = fopen('cal.data', 'w');
fprintf(fid, '%d %d %d\n', col, row, 1);
for i=1:col
    fprintf(fid,'%f ',Scores(:,i));
    fprintf(fid, '\n');
    fprintf(fid, '%f\n', calY(i));
end
fclose(fid);
