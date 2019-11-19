function [Scores,Loads, Weights,b,Score_length, centerSpecData , centerCompValue , mdt,nndt, sec ,cr] = PLS1Train(X,Y,Factor,isMix)
%PLS1校正计算
%input 
% X, 光谱矩阵，每列表示一条光谱
% Y, 性质，一行
%output
% Scores, 得分矩阵
% Loads, 载荷矩阵
% Weights, 权重矩阵
% b, 偏移量
% Score_length, 
% centerSpecData,光谱均值
% centerCompValue, 性质均值
    nanidx = isnan(Y);
    Y(nanidx) = [];
    X(:,nanidx) = [];

    cx = X;
	%X=X';Y=Y';
	centerSpecData = mean(X');
	centerCompValue = mean(Y);
	if isMix<1
		X = X -  centerSpecData'* ones(1, size(X,2)) ;
	end
	y = Y - centerCompValue;
	
	[numRowsX,numColsX] = size(X);    %光谱的波长数和样品数
	
    
	if isMix>0
		[Scores,Loads,Weights,b,Score_length] = plsmix(X,y,Factor);
	else
		[Scores,Loads, Weights,b , Score_length] = pls1(X,y,Factor);
	end
	
    [Ylast,SR,MD,nd,XScores] = PLS1Predictor(cx,Scores, Loads, Weights, b,  Score_length, centerSpecData, centerCompValue ,isMix);

    nndt = zeros(1,Factor);
    mdt = zeros(1,Factor);
    sec = zeros(1,Factor);
	cr = zeros(1,Factor);
    for i=1:Factor
        ye=Ylast(:,i)- Y;
		yea = Ylast(:,i) - centerCompValue;
		cr(i) = sumsqr(yea)/(sumsqr(yea)+sumsqr(ye)); 
        sec(i)=sqrt(ye'*ye/(numColsX-1));%计算预测标准偏差
        mdt(i) = 3*i/numColsX;
        nn = nndr(XScores(:,1:i)',XScores(:,1:i)');
        nndt(i) = max(nn);
    end

	
end