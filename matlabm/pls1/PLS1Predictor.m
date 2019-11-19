function [Ylast,SR,MD,nd,XScores] = PLS1Predictor(X,Scores, Loads, Weights, b,  Score_length, centerSpecData, centerCompValue,isMix )

    if isMix<1
		X = X -  centerSpecData'* ones(1, size(X,2)) ;
	end
	[RowsX,ColsX] = size (X);
	[RowsLoads,ColsLoads]=size(Loads); 

	% ��ʼ������������� ���о������Ϊk �������Ϊn
	tempEstimationY= zeros (ColsX,1);
	EstimationY= zeros (ColsX,ColsLoads);
	XScores =zeros (ColsX,ColsLoads);
	Factor= ColsLoads;
	
	% �����ǲ��� ASTM �ϵ�11ҳע�ͽ��У��ܹ�˳����ɶ� δ֪������Ԥ�� �������޸ĺ�ͨ�� Scores ���й���
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