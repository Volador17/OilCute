function [Scores,Loads, Weights,b , Score_length] = pls1(X,y,Factor)

    [numRowsX,numColsX] = size(X);
    Score_length = zeros(1,Factor);
    Weights = zeros(numRowsX,Factor);
    Scores = zeros(numColsX, Factor);
    b = zeros(Factor,1);
    Loads = zeros(numRowsX,Factor);
	for i = 1 : Factor 
		Weights(:,i) = X*y;                 
		Scores(:,i) = X'*Weights(:,i);        
		Score_length(i)=sqrt(Scores(:,i)'* Scores(:,i));
		Weights(:,i)=Weights(:,i)*Score_length(i);
		Scores(:,i)=Scores(:,i)/Score_length(i); 
		b(i)=Scores(:,i)'*y;                  
		Loads(:,i) = X*Scores(:,i);          
	   
		X = X - Loads(:,i)*Scores(:,i)';                   
		y = y - b(i).*Scores(:,i);
	end
	b = b';
end