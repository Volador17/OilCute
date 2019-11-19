function [Ylast, SR, MD, nd] = PLS1CrossValidation(X,Y,Factor,isMix)
 
    nanidx = isnan(Y);
    Y(nanidx) = [];
    X(:,nanidx) = [];

    [pointCount,sampleCount] = size(X);
	Ylast = zeros(sampleCount, Factor);
	SR = zeros(sampleCount, Factor);
	MD = zeros(sampleCount, Factor);
	nd = zeros(sampleCount, Factor);
    
    
    Weights = zeros(pointCount,Factor);
    Loads = zeros(pointCount,Factor);
    Scores = zeros(sampleCount-1,Factor);
    XScores = zeros(Factor);
	for m=1:sampleCount
		calX = [X(:,1:m-1) X(:,m+1:end)]; 
		calY = [Y(1:m-1);Y(m+1:end,:)];
		valX = X(:,m);  
        
        centerSpecData = mean(calX');
        centerCompValue = mean(calY);

		if isMix<1
			calX = calX - centerSpecData'* ones(1, size(calX,2)) ; 
			valX = valX - centerSpecData'* ones(1, size(valX,2));
		end
		calY = calY - ones(size(calY,1),1) * centerCompValue;
		
        for i = 1 : Factor
        %∑÷Ω‚
            if isMix>0
				ww=(calX*calY/(calY'*calY));
				Weights(:,i)=(ww/norm(ww))';
				Scores(:,i)=calX'*Weights(:,i)/(Weights(:,i)'*Weights(:,i));
				pp=(calX*Scores(:,i)/(Scores(:,i)'*Scores(:,i)));
				Loads(:,i)=(pp/norm(pp))';
				b = Scores(:,i)'*calY/(Scores(:,i)'*Scores(:,i));
				calX=calX-Loads(:,i)*Scores(:,i)';
				calY = calY-b*Scores(:,i); 
				
				%‘§≤‚
				XScores(i)= valX'*Weights(:,i);  
				if (i ==1)
					Ylast(m,i) = XScores(1)*b + centerCompValue;
				else
					Ylast(m,i) = Ylast(m,i-1)+ XScores(i)*b;
				end
			else
				Weights(:,i) = calX*calY;               
				Scores(:,i) =calX'*Weights(:,i);        
				Score_length=sqrt(Scores(:,i)'* Scores(:,i));
				Scores(:,i)=Scores(:,i)/Score_length; 
				b=Scores(:,i)'*calY;                  
				Loads(:,i) = calX*Scores(:,i); 
				calX = calX - Loads(:,i)*Scores(:,i)';  
				%‘§≤‚
				XScores(i)= valX'*Weights(:,i);   
				XScores(i)=XScores(i)/Score_length; 
				if (i ==1)
					Ylast(m,i) = XScores(1)*b + centerCompValue;
				else
					Ylast(m,i) =Ylast(m,i-1)+ XScores(i)*b;
				end
			end

            

            valX =valX-Loads(:,i)*XScores(i)';
            SR(m,i) = (sum((valX).^2))^0.5;
            %calXres(i)=sum(sum(SpecRes(:,i).^2));


            

           %yecv(m,i)= EstimationY(m,i)- Y(m);

            %º∆À„¬Ì œæ‡¿Î
            MD(m,i) = XScores(1:i)*XScores(1:i)';
            nd(:,i)=nndr(XScores(1:i)',Scores(:,1:i)');

            

        end
        
       
    end
    tempnan = zeros(1,Factor);
    tempnan(:) = NaN;

    for i=1:length(nanidx)
        if nanidx(i)
            Ylast = InsertRow(Ylast,tempnan,i);
            SR = InsertRow(SR,tempnan,i);
            MD = InsertRow(MD,tempnan,i);
            nd = InsertRow(nd,tempnan,i);
        end
    end
end




