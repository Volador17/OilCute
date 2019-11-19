loadCalData; % ����cx cy

% PLS1
Factor = 10;
[Scores, Loads, Weights,b,Score_length, centerSpecData , centerCompValue ] = PLS1Train(cx,cy,Factor);

% ANN ����
hm = 5;             % �����ڵ���
f1 = 'logsig';      % ��һ�㴫�ݺ�����ȡֵΪtansig,logsig,purelin�е�һ��
f2 = 'purelin';     % �ڶ��㴫�ݺ�����ȡֵΪtansig,logsig,purelin�е�һ��
trainf = 'trainlm'; % ѵ��������ȡֵΪtraingd��traingdm����trainbfg��trainlm�е�һ��
tn = 100;           % ѵ������
traino = 1.000000000000000e-04;% ѵ��Ŀ��

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

% �� Scores��calY д���ļ�����FANNʹ��
[row, col] = size(Scores);
fid = fopen('cal.data', 'w');
fprintf(fid, '%d %d %d\n', col, row, 1);
for i=1:col
    fprintf(fid,'%f ',Scores(:,i));
    fprintf(fid, '\n');
    fprintf(fid, '%f\n', calY(i));
end
fclose(fid);
