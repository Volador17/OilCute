function [Ylast,SR,MD,nd,XScores] = PLS1Predictor(X,Scores, Loads, Weights, b,  Score_length, centerSpecData, centerCompValue,isMix )

    if isMix<1
		X = X -  centerSpecData'* ones(1, size(X,2)) ;
	end
	[RowsX,ColsX] = size (X);
	[RowsLoads,ColsLoads]=size(Loads); 

	% 初始化分析结果矩阵 其中矩阵的行为k 矩阵的列为n
	tempEstimationY= zeros (ColsX,1);
	EstimationY= zeros (ColsX,ColsLoads);
	XScores =zeros (ColsX,ColsLoads);
	Factor= ColsLoads;
	
	% 下面是采用 ASTM 上第11页注释进行，能够顺利完成对 未知样本的预测 。经过修改后通过 Scores 进行估计
	for i = 1 : Factor
		if isMix>0
			XScores(:,i)= X' * Weights(:,i);
		else
			Weights(:,i)=Weights(:,i)/Score_length(i);
			XScores(:,i)= X'*Weights(:,i);
			XScores(:,i)=XScores(:,i)/Score_length(i);
		end
		X = X-  Loads(:,i)*XScores(:,i)';  
		MD(:,i)=diag(XScores*XScores');
		SR(i,:)=(sum((X).^2)).^0.5;
		tempEstimationY = tempEstimationY + XScores(:,i)*b(i);
		EstimationY(:,i) = tempEstimationY;
		nd(:,i)=nndr(XScores(:,1:i)',Scores(:,1:i)');
	end
	SR=SR';
	Ylast = EstimationY+centerCompValue;
end